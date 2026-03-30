using System;
using System.Text;
using System.Security.Cryptography;
using LocalGuide.Models;

namespace LocalGuide.Services
{
    /// <summary>
    /// </summary>
    public class UserAuth
    {
        // SonarQube Fix: Видалено невикористане поле WorkFactor

        /// <summary>Secret key used to sign JWT tokens (must be stored in environment variable in production).</summary>
        private readonly string _jwtSecret;

        /// <summary>
        /// Initialises a new instance of <see cref="UserAuth"/>.
        /// </summary>
        /// <param name="jwtSecret">The JWT signing secret. Must not be null or empty.</param>
        /// <exception cref="ArgumentNullException">Thrown when jwtSecret is null or empty.</exception>
        public UserAuth(string jwtSecret)
        {
            if (string.IsNullOrWhiteSpace(jwtSecret))
                throw new ArgumentNullException(nameof(jwtSecret), "JWT secret must not be empty.");
            _jwtSecret = jwtSecret;
        }

        /// <summary>
        /// Hashes a plain-text password using a salted SHA-256 approach
        /// </summary>
        /// <param name="plainPassword">The plain-text password to hash.</param>
        /// <returns>A Base64-encoded hash string containing salt and hash.</returns>
        /// <exception cref="ArgumentNullException">Thrown when plainPassword is null.</exception>
        public string HashPassword(string plainPassword)
        {
            if (plainPassword == null) throw new ArgumentNullException(nameof(plainPassword));

            // Generate a cryptographically secure 16-byte salt
            var saltBytes = new byte[16];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(saltBytes);

            var passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
            var combined = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combined, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combined, saltBytes.Length, passwordBytes.Length);

            using var sha = SHA256.Create();
            var hashBytes = sha.ComputeHash(combined);

            // Encode salt:hash as Base64 for storage
            return Convert.ToBase64String(saltBytes) + ":" + Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="plainPassword">The plain-text password to verify.</param>
        /// <param name="storedHash">The stored hash string produced by <see cref="HashPassword"/>.</param>
        /// <returns>True if the password matches; false otherwise.</returns>
        public bool VerifyPassword(string plainPassword, string storedHash)
        {
            if (string.IsNullOrWhiteSpace(plainPassword) || string.IsNullOrWhiteSpace(storedHash))
                return false;

            var parts = storedHash.Split(':');
            if (parts.Length != 2) return false;

            var saltBytes = Convert.FromBase64String(parts[0]);
            var storedBytes = Convert.FromBase64String(parts[1]);

            var passwordBytes = Encoding.UTF8.GetBytes(plainPassword);
            var combined = new byte[saltBytes.Length + passwordBytes.Length];
            Buffer.BlockCopy(saltBytes, 0, combined, 0, saltBytes.Length);
            Buffer.BlockCopy(passwordBytes, 0, combined, saltBytes.Length, passwordBytes.Length);

            using var sha = SHA256.Create();
            var computedBytes = sha.ComputeHash(combined);

            // Constant-time comparison to prevent timing attacks
            return CryptographicOperations.FixedTimeEquals(computedBytes, storedBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="user">The authenticated user for whom to generate the token.</param>
        /// <returns>A Base64-encoded token string.</returns>
        /// <exception cref="ArgumentNullException">Thrown when user is null.</exception>
        public string GenerateToken(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));

            // Payload: userId|email|role|issuedAt (UTC)
            var payload = $"{user.Id}|{user.Email}|{user.Role}|{DateTime.UtcNow:O}";
            var keyBytes = Encoding.UTF8.GetBytes(_jwtSecret);

            using var hmac = new HMACSHA256(keyBytes);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);
            var signatureBytes = hmac.ComputeHash(payloadBytes);

            return Convert.ToBase64String(payloadBytes) + "." + Convert.ToBase64String(signatureBytes);
        }

        /// <summary>
        /// </summary>
        /// <param name="token">The token string to validate.</param>
        /// <param name="userId">When this method returns true, contains the authenticated user ID.</param>
        /// <returns>True if the token is valid and not tampered with; false otherwise.</returns>
        public bool ValidateToken(string token, out int userId)
        {
            userId = 0;
            if (string.IsNullOrWhiteSpace(token)) return false;

            var parts = token.Split('.');
            if (parts.Length != 2) return false;

            try
            {
                var payloadBytes = Convert.FromBase64String(parts[0]);
                var signatureBytes = Convert.FromBase64String(parts[1]);
                var keyBytes = Encoding.UTF8.GetBytes(_jwtSecret);

                using var hmac = new HMACSHA256(keyBytes);
                var expectedSignature = hmac.ComputeHash(payloadBytes);

                // Verify signature
                if (!CryptographicOperations.FixedTimeEquals(expectedSignature, signatureBytes))
                    return false;

                // Decode payload and extract userId
                var payload = Encoding.UTF8.GetString(payloadBytes);
                var fields = payload.Split('|');
                if (fields.Length < 4) return false;

                return int.TryParse(fields[0], out userId);
            }
            catch
            {
                return false;
            }
        }
    }
}