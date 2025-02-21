using MyVideoEditor.DTOs;
using MyVideoEditor.Forms;
using MyVideoEditor.VideoObjects;

namespace MyVideoEditor.Services
{
    public class ProjectService
    {
        #region Props 

        StreamContainerService MediaContainerService { get; }
        TimelineService TimelineService { get; }
        TimeStampService TimeStampService { get; }
        public MainForm MainForm { get; }
        public FfmpegExecuteblesPaths FfmpegExecuteblesPaths { get; }

        public ProjectService(
            MainForm mainForm,
            FfmpegExecuteblesPaths ffmpegExecuteblesPaths,
            StreamContainerService mediaContainerService,
            TimelineService timelineService,
            TimeStampService timeStampService)
        {
            MainForm = mainForm;
            FfmpegExecuteblesPaths = ffmpegExecuteblesPaths;
            MediaContainerService = mediaContainerService;
            TimelineService = timelineService;
            TimeStampService = timeStampService;
        }

        #endregion

        public bool Close()
        {
            if (HasProjectFile())
            {
                SaveProjectClicked();
                return true;
            }
            else
            {
                var dialog = new SaveFileDialog();
                dialog.Filter = "*.wjvproj";
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SaveProject(dialog.FileName);
                    return true;
                }
            }
            return false;
        }

        private string[] GetDragAndDropFiles(DragEventArgs e)
        {
            if (e == null || e.Data == null)
                return Array.Empty<string>();

            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return Array.Empty<string>();

            var filesobj = e.Data.GetData(DataFormats.FileDrop);
            if (filesobj == null)
                return Array.Empty<string>();

            var files = (string[])filesobj;
            if (files == null)
                return Array.Empty<string>();

            return files;
            //return MediaContainerService.Open(files);
        }

        public bool HasProjectFile()
        {
            throw new NotImplementedException();
        }
        public void SaveProjectClicked()
        {
            throw new NotImplementedException();
        }
        public void SaveProject(string fileName)
        {
            throw new NotImplementedException();
        }

        public void DragEnter(object sender, DragEventArgs e)
        {
            var files = GetDragAndDropFiles(e);
            if (files.Any())
                e.Effect = DragDropEffects.Copy;
        }
        public void DragDrop(object sender, DragEventArgs e, Project project)
        {
            var files = GetDragAndDropFiles(e);
            if (files.Any())
            {
                InsertVideos(project, files);
            }
        }

        public Project NewProjectClicked_AfterConfirm()
        {
            var defaultTimeline = new Timeline()
            {
                Name = "Default",
            };
            return new Project()
            {
                Timelines = new List<Timeline>() { defaultTimeline },
                Medias = new List<DTOs.Media>(),
                Width = 1920,
                Height = 1080,
                FramerateBase = 60,
                FramerateDivider = 1,
                CurrentTimelineId = defaultTimeline.Id
            };
        }
        public Project OpenProjectClicked_AfterConfirm()
        {
            throw new NotImplementedException();
        }

        public void SaveAsProjectClicked()
        {
            throw new NotImplementedException();
        }

        public void InsertVideosButtonClicked(Project project)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Video Files|*.mp4;*.avi;*.mkv;*.mov;*.wmv|All Files|*.*";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                InsertVideos(project, openFileDialog.FileNames);
            }
        }
        private void InsertVideos(Project project, string[] files)
        {
            if (project == null) return;
            var timeline = project.Timelines.FirstOrDefault(a => a.Id == project.CurrentTimelineId);
            if (timeline == null) return;

            var streamContainers = MediaContainerService.Open(files);
            if (streamContainers.Any())
            {
                var timelinestart = timeline.TotalLength;

                foreach (var streamContainer in streamContainers)
                {
                    var groupid = Guid.NewGuid();
                    var duration = 0d;
                    var fullname = streamContainer.FullName;

                    var media = new Media(streamContainer);
                    project.Medias.Add(media);

                    var videosteams = streamContainer.VideoInfos
                        .Select(a => new VideoStreamReader(FfmpegExecuteblesPaths, fullname, a));
                    foreach (var videostream in videosteams)
                    {
                        if (duration == 0d)
                        {
                            if (videostream.StreamInfo.Duration == null) throw new Exception("Duration cannot be read");
                            duration = videostream.StreamInfo.Duration.Value;
                        }

                        // Blauwdruk aanmaken
                        var mediavideo = new MediaVideo()
                        {
                            MediaId = media.Id,
                            StreamIndex = videostream.StreamInfo.Index,
                            Duration = videostream.StreamInfo.Duration,
                            Resolution = videostream.StreamInfo.Resolution,
                            FramerateBase = videostream.StreamInfo.FramerateBase,
                            FramerateDivider = videostream.StreamInfo.FramerateDivider
                        };
                        media.Videos.Add(mediavideo);

                        var videoitem = new TimelineVideo()
                        {
                            GroupId = groupid,
                            MediaVideoId = media.Id,
                            TimelineStartTime = timelinestart,
                            TimelineEndTime = timelinestart + duration,
                            MediaStartTime = 0,
                            MediaEndTime = duration,
                        };
                        timeline.TimelineVideos.Add(videoitem);

                        // Control aanmaken
                        MainForm.MainTimelineControl.TimelineControl.AddTimelineVideoControl(mediavideo, videoitem, videostream);
                    }
                    foreach (var audiostream in streamContainer.AudioStreams)
                    {
                        if (duration == 0d)
                        {
                            if (audiostream.StreamInfo.Duration == null) throw new Exception("Duration cannot be read");
                            duration = audiostream.StreamInfo.Duration.Value;
                        }

                        var mediaaudio = new MediaAudio()
                        {
                            MediaId = media.Id,
                            StreamIndex = audiostream.StreamInfo.Index,
                            Duration = audiostream.StreamInfo.Duration,
                        };
                        media.Audios.Add(mediaaudio);

                        var audioitem = new TimelineAudio()
                        {
                            GroupId = groupid,
                            MediaAudioId = media.Id,
                            TimelineStartTime = timelinestart,
                            TimelineEndTime = timelinestart + duration,
                            MediaStartTime = 0,
                            MediaEndTime = duration,
                        };
                        timeline.TimelineAudios.Add(audioitem);

                        // Control aanmaken
                        MainForm.MainTimelineControl.TimelineControl.AddTimelineAudioControl(mediaaudio, audioitem, audiostream);
                    }

                    timelinestart += duration;
                }

                timeline.TotalLength = timelinestart;

                MainForm.MainTimelineControl.TimelineControl.Invalidate();
            }
        }
    }
}
