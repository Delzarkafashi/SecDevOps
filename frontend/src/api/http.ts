import { getToken } from "../auth/authStore";

const API_BASE = (import.meta.env.VITE_API_URL as string | undefined) ?? "";

type HttpMethod = "GET" | "POST" | "PUT" | "DELETE";

export async function http<T>(path: string, method: HttpMethod, body?: unknown): Promise<T> {
  const token = getToken();

  const res = await fetch(`${API_BASE}${path}`, {
    method,
    headers: {
      "Content-Type": "application/json",
      ...(token ? { Authorization: `Bearer ${token}` } : {})
    },
    body: body ? JSON.stringify(body) : undefined
  });

  if (res.status === 204) return undefined as T;

  const text = await res.text();
  const data = text ? safeJson(text) : null;

  if (!res.ok) {
    const msg =
      (typeof data === "string" && data) ||
      (data && typeof data === "object" && "message" in data && String((data as any).message)) ||
      (text || `HTTP ${res.status}`);
    throw new Error(msg);
  }

  return data as T;
}

function safeJson(text: string) {
  try {
    return JSON.parse(text);
  } catch {
    return text;
  }
}