export interface RegisterStudentRequest {
  name: string;
  email: string;
  password: string;
  department: string;
  age: number;
  cgpa: number;
}

export interface LoginRequest {
  email: string;
  password: string;
}

export interface LoginResponse {
  token: string;
  role: string;
  name: string;
  email: string;
  expiresAt: string;
}

export interface ApiMessage {
  message: string;
}
