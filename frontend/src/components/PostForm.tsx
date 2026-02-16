import { useEffect, useState } from "react";

type Props = {
  mode: "create" | "edit";
  initialTitle?: string;
  initialContent?: string;
  onSubmit: (title: string, content: string) => Promise<void> | void;
  onCancel?: () => void;
};

export function PostForm({ mode, initialTitle = "", initialContent = "", onSubmit, onCancel }: Props) {
  const [title, setTitle] = useState(initialTitle);
  const [content, setContent] = useState(initialContent);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    setTitle(initialTitle);
    setContent(initialContent);
  }, [initialTitle, initialContent]);

  async function submit(e: React.FormEvent) {
    e.preventDefault();
    setError(null);

    if (!title.trim() || !content.trim()) {
      setError("Titel och content krävs");
      return;
    }

    setLoading(true);
    try {
      await onSubmit(title.trim(), content.trim());
      if (mode === "create") {
        setTitle("");
        setContent("");
      }
    } catch (err: any) {
      setError(err?.message ?? "Fel");
    } finally {
      setLoading(false);
    }
  }

  return (
    <form className="form" onSubmit={submit}>
      <input className="input" value={title} onChange={(e) => setTitle(e.target.value)} placeholder="Titel" />
      <textarea
        className="textarea"
        value={content}
        onChange={(e) => setContent(e.target.value)}
        placeholder="Content"
        rows={4}
      />

      {error ? <div className="alert">{error}</div> : null}

      <div className="row">
        <button className="btn btnPrimary" type="submit" disabled={loading}>
          {mode === "create" ? "Skapa" : "Spara"}
        </button>

        {onCancel ? (
          <button className="btn btnSecondary" type="button" onClick={onCancel} disabled={loading}>
            Avbryt
          </button>
        ) : null}
      </div>
    </form>
  );
}