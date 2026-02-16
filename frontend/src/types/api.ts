export type Role = "viewer" | "staff" | "admin";

export type LoginRequest = {
  identifier: string;
  password: string;
};

export type LoginResponse = {
  token: string;
  role: Role;
};

export type PostDto = {
  id: number;
  title: string;
  content: string;
  created_At: string;
  updated_At: string | null;
  created_By: number;
  createdByEmail: string;
};

export type PostCreateRequest = {
  title: string;
  content: string;
};

export type PostUpdateRequest = {
  title: string;
  content: string;
};