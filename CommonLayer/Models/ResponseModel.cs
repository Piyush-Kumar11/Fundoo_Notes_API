using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLayer.Models
{
    // The ResponseModel<T> class is a generic model used to standardize API responses.
    // It helps encapsulate the success status, message, and data returned by the API.
    public class ResponseModel<T>
    {
        // Indicates whether the operation was successful or not.
        public bool Success { get; set; }

        // Holds a message (e.g., success or error message) to provide more context about the operation.
        public string Message { get; set; }

        // Contains the actual data being returned (can be of any type, defined by T).
        public T Data { get; set; }
    }
}
