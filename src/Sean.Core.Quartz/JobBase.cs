using System;
using System.Threading.Tasks;
using Quartz;

namespace Sean.Core.Quartz
{
    public abstract class JobBase : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                BeforeExecute(context);

                await ExecuteJob(context);
            }
            catch (Exception ex)
            {
                OnException(context, ex);
            }
            finally
            {
                AfterExecute(context);
            }
        }

        protected abstract Task ExecuteJob(IJobExecutionContext context);

        protected virtual void BeforeExecute(IJobExecutionContext context)
        {

        }

        protected virtual void AfterExecute(IJobExecutionContext context)
        {

        }

        protected virtual void OnException(IJobExecutionContext context, Exception exception)
        {

        }
    }
}
