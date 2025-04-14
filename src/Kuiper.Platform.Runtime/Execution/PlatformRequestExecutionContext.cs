// <copyright file="PlatformRequestExecutionContext.cs" company="Kuiper Microsystems, LLC">
// © Kuiper Microsystems, LLC. All rights reserved.
// Unauthorized copying or use of this file, via any medium, is strictly prohibited.
// For licensing inquiries, contact licensing@kuipersys.com
// </copyright>

namespace Kuiper.Platform.Runtime.Execution
{
    using Kuiper.Platform.Sdk;
    using Kuiper.Platform.Sdk.Extensibility;

    public sealed class PlatformRequestExecutionContext : IInternalExecutionContext
    {
        private PlatformRequest Request { get; set; }

        public PlatformRequestExecutionContext()
            : this(string.Empty)
        {
        }

        public PlatformRequestExecutionContext(string message, CancellationToken cancellationToken = default)
        {
            this.Message = message;
            this.ActivityId = Guid.NewGuid();
            this.CancellationToken = cancellationToken;
        }

        internal PlatformRequestExecutionContext(CancellationToken cancellationToken = default)
        {
            this.ActivityId = Guid.NewGuid();
            this.CancellationToken = cancellationToken;
        }

        internal PlatformRequestExecutionContext(PlatformRequest request, CancellationToken cancellationToken = default)
            : this(cancellationToken)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            this.Request = request;
            this.Message = request.Message;

            // Add any additional extension data into InputParameters
            if (this.Request.InputParameters != null && this.Request.InputParameters.Count > 0)
            {
                foreach (var entry in this.Request.InputParameters)
                {
                    this.InputParameters[entry.Key] = entry.Value;
                }
            }
        }

        public string Message { get; internal set; }

        public OperationStep Step { get; internal set; } = OperationStep.PreOperation;

        public CancellationToken CancellationToken { get; internal set; }

        public Guid ActivityId { get; internal set; } = Guid.NewGuid();

        public IDictionary<string, object> InputParameters { get; internal set; }
            = new Dictionary<string, object>();

        public IDictionary<string, object> OutputParameters { get; internal set; }
            = new Dictionary<string, object>();

        public IExecutionContext? ParentContext { get; internal set; }

        public IDictionary<string, object> SharedVariables { get; internal set; }
            = new Dictionary<string, object>();

        public PlatformResponse ToPlatformResponse()
        {
            var response = new PlatformResponse
            {
                Message = this.Message,
                RequestId = this.Request.RequestId,
            };

            response.ExtensionData.Add("$type", $"{this.Message}Response");

            // Optionally, you can include extension data from OutputParameters
            foreach (var output in this.OutputParameters.Where(p => p.Key != "status"))
            {
                response.ExtensionData[output.Key] = output.Value;
            }

            if (this.OutputParameters.TryGetValue("status", out var status))
            {
                response.Status = (RequestStatus)status;
            }

            return response;
        }

        public void SetStep(OperationStep step)
        {
            this.Step = step;
        }
    }
}
