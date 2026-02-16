import type { LoginRequest, LoginResponse } from "../types/api";
import { http } from "./http";

export function login(req: LoginRequest) {
  return http<LoginResponse>("/api/auth/login", "POST", req);
}