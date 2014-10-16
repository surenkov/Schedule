using System.Linq;
using System.Windows;
using System.Data.Entity;
using System.Collections.Generic;
using Schedule.Models.DataLayer;
using Schedule.Utils.Conflicts;
using Schedule.Models.ViewModels;

namespace Schedule.Windows
{
    public partial class ConflictsWindow : Window
    {
        private ConflictsManager manager;
        public static readonly RoutedEvent UpdateEvent =
            EventManager.RegisterRoutedEvent("Update", RoutingStrategy.Direct,
                typeof(RoutedEventHandler), typeof(ConflictsWindow));

        public event RoutedEventHandler Update
        {
            add { AddHandler(UpdateEvent, value); }
            remove { RemoveHandler(UpdateEvent, value); }
        }

        public ConflictsWindow()
        {
            InitializeComponent();
            manager = new ConflictsManager();

            Check();
        }

        private void Check()
        {
            ConflictsView.Items.Clear();
            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                var models = manager.TreeModels(ctx.Schedule);
                foreach (var model in models)
                    ConflictsView.Items.Add(model);
            }
        }

        private void UpdateParent()
        {
            RaiseEvent(new RoutedEventArgs(UpdateEvent));
        }

        private void ConflictsWindow_Close(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void RemoveAll_Click(object sender, RoutedEventArgs e)
        {
            var list = Models(ConflictsView.Items.OfType<ConflictsViewModel>())
                .Where(m => m.Item != null)
                .Select(m => (m.Item as Conflict).Schedule)
                .Distinct();

            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                foreach (var item in list)
                    ctx.Entry(item).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
            UpdateParent();
            Check();
        }

        private IEnumerable<ConflictsViewModel> Models(ConflictsViewModel model)
        {
            if (model != null)
            {
                var models = model.Children as IEnumerable<ConflictsViewModel>;
                foreach (var m in model.Children)
                    models = models.Union(Models(m));
                return models;
            }
            return null;
        }

        private IEnumerable<ConflictsViewModel> Models(IEnumerable<ConflictsViewModel> models)
        {
            List<ConflictsViewModel> list = new List<ConflictsViewModel>();
            list.AddRange(models);

            foreach (var model in models)
                list.AddRange(Models(model));
            return list;
        }

        private void RemoveSelected_Click(object sender, RoutedEventArgs e)
        {
            var item = ConflictsView.SelectedItem as ConflictsViewModel;
            var list = new List<Models.Schedule>();

            if (item != null)
            {
                if (item.Item != null)
                    list.Add((item.Item as Conflict).Schedule);
                else
                {
                    list.AddRange(Models(item)
                        .Where(m => m.Item != null)
                        .Select(m => (m.Item as Conflict).Schedule)
                        .Distinct());
                }
            }

            using (ScheduleDbContext ctx = new ScheduleDbContext())
            {
                foreach (var i in list)
                    ctx.Entry(i).State = EntityState.Deleted;
                ctx.SaveChanges();
            }
            UpdateParent();
            Check();
        }
    }
}