﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using Hl7.Fhir.Model;

namespace Microsoft.Health.Fhir.Client
{
    public class FhirClientException : Exception, IDisposable
    {
        public FhirClientException(FhirResponse<OperationOutcome> response, HttpStatusCode healthCheck)
        {
            Response = response;
            HealthCheckResult = healthCheck;
        }

        public HttpStatusCode StatusCode => Response.StatusCode;

        public HttpResponseHeaders Headers => Response.Headers;

        public FhirResponse<OperationOutcome> Response { get; }

        public HttpContent Content => Response.Content;

        public OperationOutcome OperationOutcome => Response.Resource;

        public HttpStatusCode HealthCheckResult { get; private set; }

        public override string Message => FormatMessage();

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public string GetActivityId()
        {
            if (Response?.Headers != null)
            {
                if (Response.Headers.TryGetValues("X-Request-Id", out var values))
                {
                    return values.First();
                }
            }

            return "NO_FHIR_ACTIVITY_ID_FOR_THIS_TRANSACTION";
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
                Response?.Dispose();
            }
        }

        private string FormatMessage()
        {
            StringBuilder message = new StringBuilder();

            string diagnostic = OperationOutcome?.Issue?.FirstOrDefault()?.Diagnostics;
            string operationId = GetActivityId();

            message.Append(StatusCode);
            if (!string.IsNullOrWhiteSpace(diagnostic))
            {
                message.Append(": ").Append(diagnostic);
            }

            string responseInfo = "NO_RESPONSEINFO";
            try
            {
                if (Response.Content != null)
                {
                    responseInfo = Response.Content.ReadAsStringAsync().Result;
                }
            }
            catch (ObjectDisposedException)
            {
                responseInfo = "RESPONSEINFO_ALREADY_DISPOSED";
            }

            string requestInfo = "NO_REQUESTINFO";
            try
            {
                if (Response.Response.RequestMessage.Content != null)
                {
                    requestInfo = Response.Response.RequestMessage.Content.ReadAsStringAsync().Result;
                }
            }
            catch (ObjectDisposedException)
            {
                requestInfo = "REQUESTINFO_ALREADY_DISPOSED";
            }

            message.Append(" (").Append(operationId).AppendLine(")");

            message.AppendLine("==============================================");
            message.Append("Url: (").Append(Response.Response.RequestMessage?.Method.Method ?? "NO_HTTP_METHOD_AVAILABLE").Append(") ").AppendLine(Response.Response.RequestMessage?.RequestUri.ToString() ?? "NO_URI_AVAILABLE");
            message.Append("Response code: ").Append(Response.Response.StatusCode.ToString()).Append('(').Append((int)Response.Response.StatusCode).AppendLine(")");
            message.Append("Reason phrase: ").AppendLine(Response.Response.ReasonPhrase ?? "NO_REASON_PHRASE");
            message.Append("Timestamp: ").AppendLine(DateTime.UtcNow.ToString("o"));
            message.Append("Health Check Result: ").Append(HealthCheckResult.ToString()).Append('(').Append((int)HealthCheckResult).AppendLine(")");
            message.Append("Response Info: ").AppendLine(responseInfo);
            message.Append("Request Info: ").AppendLine(requestInfo);
            message.AppendLine("==============================================");

            return message.ToString();
        }
    }
}
