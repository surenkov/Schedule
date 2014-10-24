using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Schedule.Models.DataLayer;
using Schedule.Models.ViewModels.Slices;
using Schedule.Utils;
using Schedule.Utils.Filters;
using Schedule.Utils.Conflicts;

namespace Schedule.Controls.Slices
{
    public class SliceView : ScheduleView
    {
        public static readonly DependencyProperty HorizontalHeaderTypeProperty =
            DependencyProperty.Register("HorizontalHeaderType", typeof(Type), typeof(SliceView),
                new PropertyMetadata(null, HorizontalHeaderTypePropertyChangedCallback));

        public static readonly DependencyProperty VerticalHeaderTypeProperty =
            DependencyProperty.Register("VerticalHeaderType", typeof(Type), typeof(SliceView),
                new PropertyMetadata(null, VerticalHeaderTypePropertyChangedCallback));

        public static readonly DependencyProperty VerticalHeaderProperty =
            DependencyProperty.Register("VerticalHeader", typeof(IEnumerable), typeof(SliceView),
                new PropertyMetadata(null, VerticalHeaderChangedCallback));

        public static readonly DependencyProperty StartDateProperty =
            DependencyProperty.Register("StartDate", typeof(DateTime), typeof(SliceView),
                new PropertyMetadata(DateTime.Now.Date, DatesChangedCallback));

        public static readonly DependencyProperty EndDateProperty =
            DependencyProperty.Register("EndDate", typeof(DateTime), typeof(SliceView),
                new PropertyMetadata(DateTime.Now.Date.AddMonths(1), DatesChangedCallback));

        static SliceView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(SliceView), new FrameworkPropertyMetadata(typeof(SliceView)));
        }

        private static void VerticalHeaderTypePropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
                view.FillVerticalHeader();
        }

        private static void HorizontalHeaderTypePropertyChangedCallback(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
                view.FillHorizontalHeader();
        }

        private static void VerticalHeaderChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null) view.OnVerticalHeaderChanged();
        }

        private static void DatesChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var view = d as SliceView;
            if (view != null)
            {
                if (view.CheckInterval())
                {
                    Type headerType = view.HorizontalHeaderType;
                    view.HorizontalHeaderType = null;
                    view.HorizontalHeaderType = headerType;

                    headerType = view.VerticalHeaderType;
                    view.VerticalHeaderType = null;
                    view.VerticalHeaderType = headerType;
                }
            }
        }

        public IEnumerable VerticalHeader
        {
            get { return (IEnumerable)GetValue(VerticalHeaderProperty); }
            set { SetValue(VerticalHeaderProperty, value); }
        }

        public Type HorizontalHeaderType
        {
            get { return (Type)GetValue(HorizontalHeaderTypeProperty); }
            set { SetValue(HorizontalHeaderTypeProperty, value); }
        }

        public Type VerticalHeaderType
        {
            get { return (Type)GetValue(VerticalHeaderTypeProperty); }
            set { SetValue(VerticalHeaderTypeProperty, value); }
        }

        public DateTime StartDate
        {
            get { return (DateTime)GetValue(StartDateProperty); }
            set { SetValue(StartDateProperty, value); }
        }

        public DateTime EndDate
        {
            get { return (DateTime)GetValue(EndDateProperty); }
            set { SetValue(EndDateProperty, value); }
        }

        public override void UpdateView()
        {
            Update();
        }

        private void OnVerticalHeaderChanged()
        {
            Update();
        }

        protected override void OnHeaderChanged(object oldHeader, object newHeader)
        {
            base.OnHeaderChanged(oldHeader, newHeader);
            Update();
        }

        private bool CheckInterval()
        {
            int interval = Properties.Settings.Default.MaxInterval;
            bool valid = (EndDate - StartDate).Days <= interval;
            if (!valid)
            {
                MessageBox.Show(string.Format(Properties.Resources.Schedule_BigInterval, interval));
                EndDate = StartDate.AddDays(interval);
            }
            return valid;
        }

        private void Update()
        {
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                if (Header == null || VerticalHeader == null) return;

                var q = from s in ctx.Schedule.Include("Course").Include("Teacher").Include("Group").Include("Class.Building").ApplyFilters(Filters).Cast<Models.Schedule>()
                        where s.EndDate >= StartDate && s.StartDate <= EndDate
                        select s;

                HashSet<Models.Schedule> itemsList = new HashSet<Models.Schedule>();
                for (DateTime date = StartDate; date <= EndDate; date = date.AddDays(1))
                    itemsList.UnionWith(q.Where(s => date >= s.StartDate && date <= s.EndDate && (date - s.StartDate).Days % s.Interval == 0));

                var horizontalHeaderItems = Header as IEnumerable;
                var vericalHeaderItems = VerticalHeader.Cast<object>().Select(item => new SliceRowViewModel
                {
                    Header = item,
                    Items = new List<SliceCellViewModel>()
                }).ToList();

                var conflicts = new ConflictsManager().CheckAll(itemsList);
                foreach (var rowModel in vericalHeaderItems)
                {
                    foreach (var horizontalHeaderItem in horizontalHeaderItems)
                    {
                        var cell = new SliceCellViewModel
                        {
                            HorizontalValue = horizontalHeaderItem,
                            VerticalValue = rowModel.Header,
                            ScheduleView = this
                        };
                        cell.Fill(itemsList, conflicts);
                        rowModel.Items.Add(cell);
                    }
                }
                ItemsSource = vericalHeaderItems;
            }
        }
    }
}
