import { useState } from "react";
import { login } from "../api/auth";
import { useAuth } from "../auth/useAuth";

export function LoginPage() {
  const { setLogin } = useAuth();
  const [identifier, setIdentifier] = useState("");
  const [password, setPassword] = useState("");
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  async function onSubmit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);
    setLoading(true);

    try {
      const res = await login({ identifier, password });
      setLogin(res.token, res.role);
        } catch (err: any) {
        const msg = String(err?.message ?? "");

        if (msg.includes("invalid credentials")) {
            setError("Fel användarnamn eller lösenord");
        } else if (msg.includes("inactive account")) {
            setError("Kontot är inaktivt");
        } else if (msg.includes("account locked")) {
            setError("Kontot är låst. Försök igen senare");
        } else if (msg.includes("identifier and password required")) {
            setError("Fyll i både användarnamn och lösenord");
        } else {
            setError("Något gick fel. Försök igen");
        }
        } finally {
      setLoading(false);
    }
  }

  return (
    <div className="page">
      <div className="container">
        <div className="panel">
          <div className="headerRow">
            <h2 className="hTitle">Logga in</h2>
          </div>

          <form className="form" onSubmit={onSubmit}>
            <div className="stack">
              <label className="label">Identifier (email eller username)</label>
              <input
                className="input"
                value={identifier}
                onChange={(e) => setIdentifier(e.target.value)}
                autoComplete="username"
              />
            </div>

            <div className="stack">
              <label className="label">Lösenord</label>
            <input
            className="input"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            type="password"
            autoComplete="off"
            />
            </div>

            {error ? <div className="alert">{error}</div> : null}

            <div className="row">
              <button className="btn btnPrimary" type="submit" disabled={loading}>
                {loading ? "Loggar in..." : "Logga in"}
              </button>
            </div>

          </form>
        </div>
      </div>
    </div>
  );
}