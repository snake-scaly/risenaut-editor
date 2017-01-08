using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RisenautEditor
{
    public class LevelView : Control
    {
        private struct Tile
        {
            public int U { get; set; }
            public int V { get; set; }

            public Tile(int u, int v) : this()
            {
                U = u;
                V = v;
            }
        }

        private Rect level_rect;
        private ImageSource rendered_level;
        private bool paint;
        private Tile? current_tile;
        private readonly Dictionary<int, Sprite> block_map = new Dictionary<int, Sprite>();

        private const int block_width = 8;
        private const int block_height = 8;

        public LevelView()
        {
            UndoCommand = new RelayCommand(Undo, () => Level != null && Level.CanUndo);
            RedoCommand = new RelayCommand(Redo, () => Level != null && Level.CanRedo);
        }

        public static readonly DependencyProperty LevelDependencyProperty =
            DependencyProperty.Register("Level", typeof(Level), typeof(LevelView),
            new FrameworkPropertyMetadata(OnLevelDependencyPropertyChanged));

        public Level Level
        {
            get { return (Level)GetValue(LevelDependencyProperty); }
            set { SetValue(LevelDependencyProperty, value); }
        }

        public static readonly DependencyProperty BlocksProperty =
            DependencyProperty.Register("Blocks", typeof(IList<Sprite>), typeof(LevelView),
            new FrameworkPropertyMetadata(OnBlocksPropertyChanged));

        public IList<Sprite> Blocks
        {
            get { return (IList<Sprite>)GetValue(BlocksProperty); }
            set { SetValue(BlocksProperty, value); }
        }

        public static readonly DependencyProperty TileBrushProperty =
            DependencyProperty.Register("TileBrush", typeof(int), typeof(LevelView), new PropertyMetadata(), ValidateTileBrush);

        static bool ValidateTileBrush(object value)
        {
            return (int)value >= 0 && (int)value <= 255;
        }

        public int TileBrush
        {
            get { return (int)GetValue(TileBrushProperty); }
            set { SetValue(TileBrushProperty, value); }
        }

        public bool IsPreview { get; set; }

        private static void OnLevelDependencyPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var self = (LevelView)source;
            if (e.OldValue != null)
            {
                ((Level)e.OldValue).PropertyChanged -= self.OnLevelPropertyChanged;
            }
            if (e.NewValue != null)
            {
                ((Level)e.NewValue).PropertyChanged += self.OnLevelPropertyChanged;
            }
            self.Update();
            self.UndoCommand.RaiseCanExecuteChanged();
            self.RedoCommand.RaiseCanExecuteChanged();
        }

        private void OnLevelPropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            if ("CanUndo".Equals(e.PropertyName))
            {
                UndoCommand.RaiseCanExecuteChanged();
            }
            else if ("CanRedo".Equals(e.PropertyName))
            {
                RedoCommand.RaiseCanExecuteChanged();
            }
        }

        private static void OnBlocksPropertyChanged(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            var lv = (LevelView)source;
            lv.ReindexBlocks();
            lv.Update();
        }

        private void ReindexBlocks()
        {
            block_map.Clear();
            foreach (Sprite s in Blocks)
            {
                block_map[s.Index] = s;
            }
        }

        private void Update()
        {
            RenderLevel();
            InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);
            drawingContext.DrawRectangle(Brushes.Black, null, new Rect(0, 0, ActualWidth, ActualHeight));

            if (Level == null || rendered_level == null)
                return;

            drawingContext.DrawImage(rendered_level, level_rect);

            // Draw only the level if in preview mode.
            if (IsPreview)
                return;

            for (int y = 0; y < Level.Height; y++)
            {
                for (int x = 0; x < Level.Width; x++)
                {
                    int b = Level.GetTile(x, y);
                    if (!block_map.ContainsKey(b))
                    {
                        Rect rect = GetTileRect(x, y);
                        const int unknown_tile_padding = 4;
                        rect.Inflate(-unknown_tile_padding, -unknown_tile_padding);
                        if (!rect.IsEmpty)
                        {
                            drawingContext.DrawRectangle(null, new Pen(Brushes.Red, 2), rect);
                            const int tile_code_padding = 2;
                            rect.Inflate(-tile_code_padding, -tile_code_padding);
                            if (!rect.IsEmpty)
                            {
                                var str = string.Format("{0:X2}", b);
                                var ft = new FormattedText(
                                    str,
                                    CultureInfo.GetCultureInfo("en-us"),
                                    FlowDirection.LeftToRight,
                                    new Typeface("Arial"),
                                    10,
                                    Brushes.White);
                                drawingContext.DrawText(ft, rect.TopLeft);
                            }
                        }
                    }

                    if (current_tile.HasValue &&
                        ((Tile)current_tile).U == x &&
                        ((Tile)current_tile).V == y)
                    {
                        Rect rect = GetTileRect(x, y);
                        const int current_tile_padding = 2;
                        rect.Inflate(-current_tile_padding, -current_tile_padding);
                        if (!rect.IsEmpty)
                        {
                            drawingContext.DrawRectangle(null, new Pen(Brushes.LightGreen, 2), rect);
                        }
                    }
                }
            }
        }

        private Rect GetTileRect(int u, int v)
        {
            double tile_width = level_rect.Width / Level.Width;
            double tile_height = level_rect.Height / Level.Height;
            Rect rect = new Rect(
                level_rect.X + tile_width * u,
                level_rect.Y + tile_height * v,
                tile_width,
                tile_height);
            return rect;
        }

        private Rect GetLevelRect(Size render_size)
        {
            const double aspect = 4.0 / 3.0;
            if (render_size.Width / render_size.Height <= aspect)
            {
                double hh = render_size.Width / aspect;
                return new Rect(0, (render_size.Height - hh) / 2, render_size.Width, hh);
            }
            else
            {
                double ww = render_size.Height * aspect;
                return new Rect((render_size.Width - ww) / 2, 0, ww, render_size.Height);
            }
        }

        private Tile? GetTileAtPoint(Point point)
        {
            if (Level == null)
            {
                return null;
            }
            double tile_width = level_rect.Width / Level.Width;
            double tile_height = level_rect.Height / Level.Height;
            int u = (int)Math.Floor((point.X - level_rect.X) / tile_width);
            int v = (int)Math.Floor((point.Y - level_rect.Y) / tile_height);
            if (u >= 0 && u < Level.Width && v >= 0 && v < Level.Height)
            {
                return new Tile(u, v);
            }
            return null;
        }

        private void RenderLevel()
        {
            if (Level == null)
                return;

            //var canvas = new DrawingContextCanvas(drawingContext, ActualWidth, ActualHeight);
            int screen_width = Level.Width * block_width;
            int screen_height = Level.Height * block_height;
            const int bpp = 3;
            var pixels = new byte[bpp * screen_width * screen_height];
            var canvas = new Bgr24Canvas(pixels, bpp * screen_width, screen_width, screen_height);

            for (int y = 0; y < Level.Height; y++)
            {
                for (int x = 0; x < Level.Width; x++)
                {
                    int b = Level.GetTile(x, y);
                    Sprite sprite;
                    if (block_map.TryGetValue(b, out sprite))
                    {
                        sprite.Draw(canvas, x * block_width, y * block_height);
                    }
                }
            }

            var bmp = new WriteableBitmap(screen_width, screen_height, 96, 96, PixelFormats.Bgr24, null);

            bmp.Lock();
            bmp.WritePixels(new Int32Rect(0, 0, screen_width, screen_height), pixels, bpp * screen_width, 0);
            bmp.Unlock();

            rendered_level = bmp;
        }

        private void Paint(Tile t)
        {
            Level.SetTile(t.U, t.V, TileBrush);
            Update();
        }

        private void Undo()
        {
            Level.Undo();
            Update();
        }

        private void Redo()
        {
            Level.Redo();
            Update();
        }

        public RelayCommand UndoCommand { get; private set; }
        public RelayCommand RedoCommand { get; private set; }

        protected override void OnMouseDown(MouseButtonEventArgs e)
        {
            base.OnMouseDown(e);
            if (IsPreview)
            {
                return;
            }
            Focus();
            CaptureMouse();
            paint = true;
            if (current_tile.HasValue)
            {
                Paint((Tile)current_tile);
            }
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            ReleaseMouseCapture();
            paint = false;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);
            if (IsPreview)
            {
                return;
            }
            Tile? new_tile = GetTileAtPoint(e.GetPosition(this));
            if (!new_tile.Equals(current_tile))
            {
                current_tile = new_tile;
                if (paint && current_tile.HasValue)
                {
                    Paint((Tile)current_tile);
                }
                else
                {
                    InvalidateVisual();
                }
            }
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            if (IsPreview)
            {
                return;
            }
            current_tile = null;
            InvalidateVisual();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);
            level_rect = GetLevelRect(sizeInfo.NewSize);
            InvalidateVisual();
        }
    }
}
