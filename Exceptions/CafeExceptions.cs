namespace CafeManagement.Exceptions
{
    public class CafeException : Exception {
        public CafeException(string message) : base(message) {}
    }

    public class ItemNotFoundException : CafeException {
        public ItemNotFoundException(string message) : base(message) {}
    }

    public class OrderException : CafeException {
        public OrderException(string message) : base(message) {}
    }

    public class AuthException : CafeException {
        public AuthException(string message) : base(message) {}
    }

    public class DuplicateEmailException : CafeException {
        public DuplicateEmailException(string message) : base(message) {}
    }

    public class UnauthorizedCafeAccessException : CafeException {
        public UnauthorizedCafeAccessException(string message) : base(message) {}
    }
}
