export const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;

export function validateEmail(email: string): string | null {
  if (email.length === 0) {
    return 'Email is required';
  }
  if (!emailRegex.test(email)) {
    return 'Please enter a valid email';
  }
  return null;
}

export function validatePassword(password: string): string | null {
  if (password.length === 0) {
    return 'Password is required';
  }
  if (password.length < 6) {
    return 'Password must be at least 6 characters';
  }
  return null;
}

export function validateConfirmPassword(password: string, confirmPassword: string): string | null {
  if (password !== confirmPassword) {
    return 'Passwords do not match';
  }
  return null;
}
