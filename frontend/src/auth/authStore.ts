import type { Role } from "../types/api";

const TOKEN_KEY = "demo_token";
const ROLE_KEY = "demo_role";

export function getToken(): string | null {
  return sessionStorage.getItem(TOKEN_KEY);
}

export function getRole(): Role | null {
  return (sessionStorage.getItem(ROLE_KEY) as Role | null) ?? null;
}

export function setAuth(token: string, role: Role) {
  sessionStorage.setItem(TOKEN_KEY, token);
  sessionStorage.setItem(ROLE_KEY, role);
}

export function clearAuth() {
  sessionStorage.removeItem(TOKEN_KEY);
  sessionStorage.removeItem(ROLE_KEY);
}

export function getUserId(): number | null {
  const token = getToken();
  if (!token) return null;

  const payload = JSON.parse(atob(token.split(".")[1]));
  const id =
    payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"];

  return id ? Number(id) : null;
}