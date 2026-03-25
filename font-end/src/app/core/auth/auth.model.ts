/**
 * LoginRequest
 *
 * Payload used for user authentication.
 */
export interface LoginRequest {

  /**
   * User email or username used to log in
   */
  emailOrUserName: string;

  /**
   * User password (plain text before being sent to server)
   */
  password: string;
}

/**
 * RegisterRequest
 *
 * Payload used for user registration.
 */
export interface RegisterRequest {

  /**
   * User email address
   */
  email: string;

  /**
   * Unique username chosen by the user
   */
  userName: string;

  /**
   * User password
   */
  password: string;

  /**
   * Password confirmation (must match password)
   */
  confirmPassword: string;
}

/**
 * TokenResponse
 *
 * Response returned after successful authentication.
 * Contains tokens used for authorization.
 */
export interface TokenResponse {

  /**
   * JWT access token used for authenticated API requests
   */
  accessToken: string;

  /**
   * Refresh token used to obtain a new access token
   */
  refreshToken: string;
}

/**
 * UserProfile
 *
 * Represents authenticated user information.
 */
export interface UserProfile {

  /**
   * Unique user identifier
   */
  id: string;

  /**
   * User email address
   */
  email: string;

  /**
   * Username displayed in the system
   */
  userName: string;

  /**
   * List of roles assigned to the user (e.g., 'ADMIN', 'USER')
   */
  roles: string[];
}
