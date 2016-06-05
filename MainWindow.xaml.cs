using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using MergePhotos.Helpers;
using Image = System.Windows.Controls.Image;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MessageBox = System.Windows.MessageBox;
using Path = System.IO.Path;

namespace MergePhotos {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private string _currentDirectory;
        private readonly ObservableCollection<ImageDetails> _images1 = new ObservableCollection<ImageDetails>();
        private readonly ObservableCollection<ImageDetails> _images2 = new ObservableCollection<ImageDetails>();
        private readonly ObservableCollection<ImageDetails> _images3 = new ObservableCollection<ImageDetails>();
        private ObservableCollection<ImageDetails> _images = new ObservableCollection<ImageDetails>();
        private int _imgWidth = 150;
        private int _imgHeight = 100;
        private List<string> _extensions = new List<string> { ".jpg"/*, ".avi", ".mov"*/ };

        public MainWindow() {
            InitializeComponent();
            ImageList.ItemsSource = _images;
        }

        private void Brower1Click(object sender, RoutedEventArgs e) {
            foreach (var imageDetailse in _images1) {
                _images.Remove(imageDetailse);
            }
            _images1.Clear();
            if (_currentDirectory == null) {
                ControlSaveHelper.LoadValue(this, "Tag");
                _currentDirectory = (string)Tag;
            }
            if (Directory1Button.Tag != null && Directory1Button.Tag.Equals(true)) {
                if (Directory.Exists(Directory1TextBox.Text)) {
                    Tag = Directory1TextBox.Text = _currentDirectory = Directory1TextBox.Text;
                    var thread = new Thread(arg => FillList(_images1, 1));
                    thread.Start();
                    Directory1Button.Content = "Browser1";
                    Directory1Button.Tag = false;
                } else {
                    MessageBox.Show("Wrong directory");
                }
            } else {
                var dialog = new FolderBrowserDialog { SelectedPath = _currentDirectory };
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    Tag = Directory1TextBox.Text = _currentDirectory = dialog.SelectedPath;
                    var thread = new Thread(arg => FillList(_images1, 1));
                    thread.Start();
                }
            }
        }

        private void Brower2Click(object sender, RoutedEventArgs e) {
            foreach (var imageDetailse in _images2) {
                _images.Remove(imageDetailse);
            }
            _images2.Clear();
            if (Directory2Button.Tag != null && Directory2Button.Tag.Equals(true)) {
                if (Directory.Exists(Directory2TextBox.Text)) {
                    Tag = Directory2TextBox.Text = _currentDirectory = Directory2TextBox.Text;
                    var thread = new Thread(arg => FillList(_images2, 2));
                    thread.Start();
                    Directory2Button.Content = "Browser2";
                    Directory2Button.Tag = false;
                } else {
                    MessageBox.Show("Wrong directory");
                }
            } else {
                var dialog = new FolderBrowserDialog { SelectedPath = _currentDirectory };
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    Tag = Directory2TextBox.Text = _currentDirectory = dialog.SelectedPath;
                    var thread = new Thread(arg => FillList(_images2, 2));
                    thread.Start();
                }
            }
        }

        private void Brower3Click(object sender, RoutedEventArgs e) {
            foreach (var imageDetailse in _images3) {
                _images.Remove(imageDetailse);
            }
            _images3.Clear();
            if (Directory3Button.Tag != null && Directory3Button.Tag.Equals(true)) {
                if (Directory.Exists(Directory3TextBox.Text)) {
                    Tag = Directory3TextBox.Text = _currentDirectory = Directory3TextBox.Text;
                    var thread = new Thread(arg => FillList(_images3, 3));
                    thread.Start();
                    Directory3Button.Content = "Browser3";
                    Directory3Button.Tag = false;
                } else {
                    MessageBox.Show("Wrong directory");
                }
            } else {
                var dialog = new FolderBrowserDialog { SelectedPath = _currentDirectory };
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
                    Tag = Directory3TextBox.Text = _currentDirectory = dialog.SelectedPath;
                    var thread = new Thread(arg => FillList(_images3, 3));
                    thread.Start();
                }
            }
        }
        private void FillList(ObservableCollection<ImageDetails> images, byte sourceIndex) {
            var start = DateTime.Now;
            var delteDatetime = new long[] { 0 };
            var files = Directory.GetFiles(_currentDirectory);
            for (int i = 0; i < files.Count(); i++) {
                var file = files[i];
                var extension = Path.GetExtension(file).ToLower();
                if (_extensions.Contains(extension) == false) {
                    continue;
                }
                if (extension == ".jpg") {
                    var fileStream = new FileStream(file, FileMode.Open, FileAccess.Read);
                    var bitmapFrame = BitmapFrame.Create(fileStream, BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                    DateTime dt = DateTime.Now;
                    if (((BitmapMetadata)bitmapFrame.Metadata).DateTaken != null) {
                        dt = DateTime.Parse(((BitmapMetadata)bitmapFrame.Metadata).DateTaken);
                    } else {
                        if (i != 0) {
                            int j = i - 1;
                            BitmapFrame bitmapFrameTmp = BitmapFrame.Create(new Uri(files[j - 1]));
                            for (; ((BitmapMetadata)bitmapFrameTmp.Metadata).DateTaken == null; j--) {
                                bitmapFrameTmp = BitmapFrame.Create(new Uri(files[j - 1]), BitmapCreateOptions.DelayCreation, BitmapCacheOption.None);
                            }
                            dt = DateTime.Parse(((BitmapMetadata)bitmapFrameTmp.Metadata).DateTaken);
                        } else {
                            throw new Exception("can`t set date");
                        }
                    }
                    fileStream.Close();
                    var camera = ((BitmapMetadata)bitmapFrame.Metadata).CameraManufacturer;
                    if (camera != null) {
                        camera = camera.Split(' ').First();
                    }
                    var fi = new FileInfo(file);
                    var id = new ImageDetails {
                        Path = file,
                        FileName = Path.GetFileName(file),
                        Extension = Path.GetExtension(file),
                        DateTimeShot = dt,
                        Size = fi.Length,
                        Width = bitmapFrame.PixelWidth,
                        Height = bitmapFrame.PixelHeight,
                        SourceIndex = sourceIndex,
                        DeltaDatetime = delteDatetime,
                        Camera = camera
                    };

                    int i1 = i;
                    Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)delegate {
                        var img = new BitmapImage();

                        img.BeginInit();
                        img.CacheOption = BitmapCacheOption.OnLoad;
                        var k = Math.Min(id.Width / _imgWidth, id.Height / _imgHeight);
                        img.DecodePixelWidth = id.Width / k;
                        img.DecodePixelHeight = id.Height / k;
                        img.UriSource = new Uri(files[i1], UriKind.Absolute);
                        img.EndInit();
                        id.Width = img.PixelWidth;
                        id.Height = img.PixelHeight;
                        id.BitmapImage = img;

                        images.Add(id); _images.Add(id); statusText.Text =
                            String.Format("({0}/{1}) {2:0.00}sec", i1 + 1, files.Count(),
                             (DateTime.Now - start).TotalSeconds * ((double)files.Count() / i1 - 1));
                    });
                }
            }
            Dispatcher.BeginInvoke(DispatcherPriority.SystemIdle, (ThreadStart)ImagesSort);
        }

        private void ImagesSort() {
            ImageList.ItemsSource = _images.OrderBy(im => im.DateTimeShot.Ticks + im.DeltaDatetime[0]).ToList();
            for (int i = 0; i < _images.Count; i++) {
                ((List<ImageDetails>)ImageList.ItemsSource)[i].Index = i + 1;
                SetRename(((List<ImageDetails>)ImageList.ItemsSource)[i]);
            }
        }

        private void SetRename(ImageDetails imageDetails) {
            var datetime = new DateTime(imageDetails.DateTimeShot.Ticks + imageDetails.DeltaDatetime[0]);
            imageDetails.FileRename = String.Format("{0:000} - {1:d MMMM yyyy}.jpg", imageDetails.Index, datetime).ToLower();
        }

        void image_MouseDown(object sender, MouseButtonEventArgs e) {
            if (e.ChangedButton == MouseButton.Left && e.ClickCount == 2) {
                System.Diagnostics.Process.Start(((BitmapImage)((Image)sender).Source).UriSource.LocalPath);
            }
        }

        private void ScrollViewer_Loaded(object sender, RoutedEventArgs e) {
            scrollViewer.AddHandler(MouseWheelEvent, new RoutedEventHandler(MyMouseWheelH), true);
        }

        private void MyMouseWheelH(object sender, RoutedEventArgs e) {
            var eargs = (MouseWheelEventArgs)e;
            double x = eargs.Delta;
            double y = scrollViewer.VerticalOffset;
            scrollViewer.ScrollToVerticalOffset(y - x);
        }

        private void LeftClick(object sender, RoutedEventArgs e) {
            if (ImageList.SelectedItem == null) {
                return;
            }
            var select = (ImageDetails)ImageList.SelectedItem;
            var list = (List<ImageDetails>)ImageList.ItemsSource;
            var selectIndex = list.IndexOf(select);
            for (int i = selectIndex - 1; i >= 0; i--) {
                if (select.SourceIndex != list[i].SourceIndex) {
                    select.DeltaDatetime[0] = -select.DateTimeShot.Ticks + (list[i].DateTimeShot.Ticks + list[i].DeltaDatetime[0]) - 1;
                    //list[i].DeltaDatetime[0] = 0;
                    break;
                }
            }
            list.Clear();
            ImagesSort();
            ImageList.Focus();
        }

        private void RightClick(object sender, RoutedEventArgs e) {
            if (ImageList.SelectedItem == null) {
                return;
            }
            var select = (ImageDetails)ImageList.SelectedItem;
            var list = (List<ImageDetails>)ImageList.ItemsSource;
            var selectIndex = list.IndexOf(select);
            for (int i = selectIndex; i < list.Count; i++) {
                if (select.SourceIndex != list[i].SourceIndex) {
                    select.DeltaDatetime[0] = -select.DateTimeShot.Ticks + (list[i].DateTimeShot.Ticks + list[i].DeltaDatetime[0]) + 1;
                    //list[i].DeltaDatetime[0] = 0;
                    break;
                }
            }
            list.Clear();
            ImagesSort();
            ImageList.Focus();
        }

        private void LLeftClick(object sender, RoutedEventArgs e) {
            if (ImageList.SelectedItem == null) {
                return;
            }
            var select = (ImageDetails)ImageList.SelectedItem;
            var list = (List<ImageDetails>)ImageList.ItemsSource;
            var selectIndex = list.IndexOf(select);
            var k = 0;
            for (int i = selectIndex - 1; i >= 0; i--) {
                if (select.SourceIndex != list[i].SourceIndex) {
                    k++;
                    if (k == 10) {
                        select.DeltaDatetime[0] = -select.DateTimeShot.Ticks + (list[i].DateTimeShot.Ticks + list[i].DeltaDatetime[0]) - 1;
                        //list[i].DeltaDatetime[0] = 0;
                        break;
                    }
                }
            }
            list.Clear();
            ImagesSort();
            ImageList.Focus();
        }

        private void RRightClick(object sender, RoutedEventArgs e) {
            if (ImageList.SelectedItem == null) {
                return;
            }
            var select = (ImageDetails)ImageList.SelectedItem;
            var list = (List<ImageDetails>)ImageList.ItemsSource;
            var selectIndex = list.IndexOf(select);
            var k = 0;
            for (int i = selectIndex; i < list.Count; i++) {
                if (select.SourceIndex != list[i].SourceIndex) {
                    k++;
                    if (k == 10) {
                        select.DeltaDatetime[0] = -select.DateTimeShot.Ticks + (list[i].DateTimeShot.Ticks + list[i].DeltaDatetime[0]) + 1;
                        //list[i].DeltaDatetime[0] = 0;
                        break;
                    }
                }
            }
            list.Clear();
            ImagesSort();
            ImageList.Focus();
        }

        private const string DATE_TAKEN_QUERY = "/app1/ifd/{ushort=306}";
        private const string ORIGINAL_DATE_QUERY = "/app1/ifd/exif/{ushort=36867}";
        private const string DIGITIZED_DATE_QUERY = "/app1/ifd/exif/{ushort=36868}";
        private const string LATITUDE_QUERY = "/app1/ifd/gps/subifd:{ulong=2}";
        private const string LONGITUDE_QUERY = "/app1/ifd/gps/subifd:{ulong=4}";
        private const string NORTH_OR_SOUTH_QUERY = "/app1/ifd/gps/subifd:{char=1}";
        private const string EAST_OR_WEST_QUERY = "/app1/ifd/gps/subifd:{char=3}";
        private const string GPS_VERSION_QUERY = "/app1/ifd/gps/";
        private const string IPTC_KEYWORDS_QUERY = "/app13/irb/8bimiptc/iptc/keywords";

        //http://social.msdn.microsoft.com/Forums/ru-RU/9a15b68a-095d-478f-a424-1684e59c3edd/-c?forum=programminglanguageru
        private void SetDateTime1(string originalFileName, string outputFileName, DateTime Date) {
            bool tryOneLastMethod = false;
            using (Stream originalFile = new FileStream(originalFileName, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                BitmapDecoder original = BitmapDecoder.Create(originalFile, createOptions, BitmapCacheOption.None);
                var output = new JpegBitmapEncoder();

                if (original.Frames[0] != null && original.Frames[0].Metadata != null) {
                    var bitmapMetadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;
                    //bitmapMetadata.SetQuery("/app1/ifd/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                    //bitmapMetadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                    //bitmapMetadata.SetQuery("/xmp/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);

                    bitmapMetadata.DateTaken = Date.ToString("M/d/yyyy HH:mm:ss");
                    bitmapMetadata.SetQuery(DATE_TAKEN_QUERY, Date.ToString("yyyy:MM:dd HH:mm:ss"));
                    bitmapMetadata.SetQuery(DIGITIZED_DATE_QUERY, Date.ToString("yyyy:MM:dd HH:mm:ss"));
                    bitmapMetadata.SetQuery(ORIGINAL_DATE_QUERY, Date.ToString("yyyy:MM:dd HH:mm:ss"));

                    output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, bitmapMetadata, original.Frames[0].ColorContexts));
                }

                try {
                    using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite)) {
                        output.Save(outputFile);
                    }
                } catch (NotSupportedException e) {
                    System.Diagnostics.Debug.Print(e.Message);
                    output = new JpegBitmapEncoder();
                    output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, original.Metadata, original.Frames[0].ColorContexts));
                    using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite)) {
                        output.Save(outputFile);
                    }
                    tryOneLastMethod = true;
                }
            }

            if (tryOneLastMethod) {
                File.Move(outputFileName, outputFileName + "tmp");
                using (Stream recentlyOutputFile = new FileStream(outputFileName + "tmp", FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    BitmapCreateOptions createOptions = BitmapCreateOptions.PreservePixelFormat | BitmapCreateOptions.IgnoreColorProfile;
                    BitmapDecoder original = BitmapDecoder.Create(recentlyOutputFile, createOptions, BitmapCacheOption.None);
                    var output = new JpegBitmapEncoder();
                    if (original.Frames[0] != null && original.Frames[0].Metadata != null) {
                        var bitmapMetadata = original.Frames[0].Metadata.Clone() as BitmapMetadata;
                        //bitmapMetadata.SetQuery("/app1/ifd/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                        //bitmapMetadata.SetQuery("/app1/ifd/exif/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);
                        //bitmapMetadata.SetQuery("/xmp/PaddingSchema:Padding", METADATA_PADDING_IN_BYTES);

                        bitmapMetadata.DateTaken = Date.ToString("M/d/yyyy HH:mm:ss");
                        bitmapMetadata.SetQuery(DATE_TAKEN_QUERY, Date.ToString("yyyy:MM:dd HH:mm:ss"));
                        bitmapMetadata.SetQuery(DIGITIZED_DATE_QUERY, Date.ToString("yyyy:MM:dd HH:mm:ss"));
                        bitmapMetadata.SetQuery(ORIGINAL_DATE_QUERY, Date.ToString("yyyy:MM:dd HH:mm:ss"));

                        output.Frames.Add(BitmapFrame.Create(original.Frames[0], original.Frames[0].Thumbnail, bitmapMetadata, original.Frames[0].ColorContexts));
                    }

                    using (Stream outputFile = File.Open(outputFileName, FileMode.Create, FileAccess.ReadWrite)) {
                        output.Save(outputFile);
                    }
                }
                File.Delete(outputFileName + "tmp");
            }
            File.Delete(originalFileName);

        }

        private void SetNamesClick(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Are you sure you want to rename files?", "Rename", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                foreach (ImageDetails t in _images) {
                    if (t.FileName == t.FileRename) {
                        continue;
                    }
                    int diffHours = 0;

                    var renameName = t.Path.Replace(t.FileName, t.FileRename);
                    if (File.Exists(renameName)) {
                        var tmpName = t.Path.Replace(t.FileName, t.FileRename.Replace(".jpg", "_tmp.jpg"));
                        File.Move(t.Path.Replace(t.FileName, t.FileRename), tmpName);
                        var img = _images.First(f => f.Path == renameName);
                        img.Path = tmpName;
                        img.FileName = img.FileName.Replace(".jpg", "_tmp.jpg");
                    }
                    try {
                        if (Int32.TryParse(DiffTextBox.Text, out diffHours)) {

                            SetDateTime1(t.Path, renameName, t.DateTimeShot.AddHours(diffHours));
                        } else {
                            File.Move(t.Path, renameName);
                        }
                    } catch (Exception exception) {
                        if (MessageBox.Show(String.Format("file: {0}\n{1}", Path.GetFileName(renameName), exception.Message), "Alert", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                            break;
                        }
                    }
                }
            }
        }

        private void SetQualityClick(object sender, RoutedEventArgs e) {
            if (MessageBox.Show("Are you sure you want to set quality files?", "Rename", MessageBoxButton.OKCancel) == MessageBoxResult.OK) {
                foreach (ImageDetails t in _images) {
                    string renameName = String.Empty;
                    try {
                        if (t.FileName == t.FileRename) {
                            File.Move(t.Path, t.Path + "_tmp");
                            t.Path += "_tmp";
                            t.FileName += "_tmp";
                        }
                        renameName = t.Path.Replace(t.FileName, t.FileRename);
                        SetDateTime1(t.Path, renameName, t.DateTimeShot);
                    } catch (Exception exception) {
                        if (MessageBox.Show(String.Format("file: {0}\n{1}", Path.GetFileName(renameName), exception.Message), "Alert", MessageBoxButton.YesNo) == MessageBoxResult.No) {
                            break;
                        }
                    }
                }
            }
        }

        private ImageCodecInfo GetEncoder(ImageFormat format) {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs) {
                if (codec.FormatID == format.Guid) {
                    return codec;
                }
            }
            return null;
        }

        private void Window_SourceInitialized(object sender, EventArgs e) {
            ControlSaveHelper.LoadValue(this, "Width");
            ControlSaveHelper.LoadValue(this, "Height");
            ControlSaveHelper.LoadValue(this, "Top");
            ControlSaveHelper.LoadValue(this, "Left");
            ControlSaveHelper.LoadValue(this, "WindowState");
        }

        private void Window_Closed(object sender, EventArgs e) {
            if (WindowState != WindowState.Minimized) {
                ControlSaveHelper.SaveValue(this, "Width");
                ControlSaveHelper.SaveValue(this, "Height");
                ControlSaveHelper.SaveValue(this, "Top");
                ControlSaveHelper.SaveValue(this, "Left");
                ControlSaveHelper.SaveValue(this, "WindowState");
                ControlSaveHelper.SaveValue(this, "Tag");
            }
        }

        private void NewDateTime_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            var select = (ImageDetails)((FrameworkElement)sender).DataContext;
            var win = new DateTimeWindow();
            win.DateTime = select.DateTimeShot;
            if (win.ShowDialog() == true) {
                select.DeltaDatetime[0] = win.DateTime.Ticks - select.DateTimeShot.Ticks;
                ImagesSort();
                ImageList.Focus();
            }
        }

        private void Directory1TextBoxTextChanged(object sender, KeyEventArgs e) {
            Directory1Button.Tag = true;
            Directory1Button.Content = "Load";
        }

        private void Directory2TextBoxTextChanged(object sender, KeyEventArgs e) {
            Directory2Button.Tag = true;
            Directory2Button.Content = "Load";
        }

        private void Directory3TextBoxTextChanged(object sender, KeyEventArgs e) {
            Directory3Button.Tag = true;
            Directory3Button.Content = "Load";
        }
    }
}
