using Harrison314.PassiveMQ.Client.Models;
using Microsoft.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client.Internal
{
    internal static class HttpOperationResponseExtensions
    {
        public static T AsResult<T>(this HttpOperationResponse<object> httpOperationResponse)
        {
            switch (httpOperationResponse.Body)
            {
                case null:
                    return default(T);

                case T response:
                    return response;

                case ErrorResponseDto error:
                    throw new PassiveMqException(error);

            }

            throw new InvalidProgramException($"Type {httpOperationResponse.Body.GetType().FullName} and status {httpOperationResponse.Response.StatusCode} is not supported.");
        }

        public static void AsResult(this HttpOperationResponse<ErrorResponseDto> httpOperationResponse)
        {
            if (httpOperationResponse.Body != null)
            {
                throw new PassiveMqException(httpOperationResponse.Body);
            }
        }

        public static void AsResult(this HttpOperationResponse<object> httpOperationResponse)
        {
            switch (httpOperationResponse.Body)
            {
                case ErrorResponseDto error:
                    throw new PassiveMqException(string.Join("; ", error.Messages));
            }
        }
    }
}
