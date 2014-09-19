using System.Data.Entity.Validation;
using System.Text;
using System.Windows;

namespace Schedule.Utils
{
    static class ScheduleDbExceptionExtender
    {
        public static void ShowMessage(this DbEntityValidationException e)
        {

            StringBuilder msg = new StringBuilder();
            msg.AppendLine("Please fix following errors:");

            foreach (DbEntityValidationResult entityErr in e.EntityValidationErrors)
                foreach (DbValidationError error in entityErr.ValidationErrors)
                    msg.AppendFormat("{0}: {1}\n", error.PropertyName, error.ErrorMessage);
            
            MessageBox.Show(msg.ToString(), "There are following errors");
        }
    }
}
