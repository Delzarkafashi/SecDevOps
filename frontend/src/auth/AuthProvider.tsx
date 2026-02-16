import React, { createContext, useEffect, useMemo, useState } from "react";
import type { Role } from "../types/api";
import { clearAuth, getRole, getToken, setAuth } from "./authStore";

type AuthState = {
  token: string | null;
  role: Role | null;
  isAuthed: boolean;
  setLogin: (token: string, role: Role) => void;
  logout: () => void;
};

export const AuthContext = createContext<AuthState | null>(null);

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [token, setToken] = useState<string | null>(null);
  const [role, setRole] = useState<Role | null>(null);

  useEffect(() => {
    setToken(getToken());
    setRole(getRole());
  }, []);

  const value = useMemo<AuthState>(
    () => ({
      token,
      role,
      isAuthed: Boolean(token),
      setLogin: (t, r) => {
        setAuth(t, r);
        setToken(t);
        setRole(r);
      },
      logout: () => {
        clearAuth();
        setToken(null);
        setRole(null);
      }
    }),
    [token, role]
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}