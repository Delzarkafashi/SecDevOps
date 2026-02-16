import { useEffect, useMemo, useState } from "react";
import { createPost, deletePost, getPosts, updatePost } from "../api/posts";
import { useAuth } from "../auth/useAuth";
import { PostForm } from "../components/PostForm";
import { PostList } from "../components/PostList";
import type { PostDto, Role } from "../types/api";

export function PostsPage() {
  const { role, logout } = useAuth();
  const [posts, setPosts] = useState<PostDto[]>([]);
  const [error, setError] = useState<string | null>(null);
  const [loading, setLoading] = useState(false);

  const [editing, setEditing] = useState<PostDto | null>(null);

  const canCreate = role === "staff" || role === "admin";
  const canEdit = role === "staff" || role === "admin";
  const canDelete = role === "admin";

  const safeRole = (role ?? "viewer") as Role;

  async function load() {
    setError(null);
    setLoading(true);
    try {
      const rows = await getPosts();
      setPosts(rows);
    } catch (err: any) {
      setError(err?.message ?? "Fel vid hämtning");
    } finally {
      setLoading(false);
    }
  }

  useEffect(() => {
    load();
  }, []);

  async function onCreate(title: string, content: string) {
    setError(null);
    try {
      await createPost({ title, content });
      await load();
    } catch (err: any) {
      setError(err?.message ?? "Fel vid skapa");
    }
  }

  async function onSaveEdit(title: string, content: string) {
    if (!editing) return;
    setError(null);
    try {
      await updatePost(editing.id, { title, content });
      setEditing(null);
      await load();
    } catch (err: any) {
      setError(err?.message ?? "Fel vid uppdatera");
    }
  }

  async function onDelete(post: PostDto) {
    if (!canDelete) return;
    const ok = confirm(`Radera post ${post.id}?`);
    if (!ok) return;

    setError(null);
    try {
      await deletePost(post.id);
      await load();
    } catch (err: any) {
      setError(err?.message ?? "Fel vid radera");
    }
  }

  const header = useMemo(() => `Inloggad roll: ${safeRole}`, [safeRole]);

  return (
    <div className="page">
      <div className="container">
        <div className="panel">
          <div className="headerRow">
            <h2 className="hTitle">Posts</h2>
            <div className="row">
              <span className="badge">{header}</span>
              <button className="btn btnSecondary" onClick={logout}>
                Logga ut
              </button>
            </div>
          </div>

          {error ? <div className="alert">{error}</div> : null}

          <div className="row">
            <button className="btn btnSecondary" onClick={load} disabled={loading}>
              {loading ? "Laddar..." : "Uppdatera lista"}
            </button>
          </div>

          {canCreate ? (
            <div className="stack" style={{ marginTop: 12 }}>
              <h3 className="sectionTitle">Skapa post</h3>
              <PostForm mode="create" onSubmit={onCreate} />
            </div>
          ) : null}

          {canEdit && editing ? (
            <div className="stack" style={{ marginTop: 12 }}>
              <h3 className="sectionTitle">Uppdatera post {editing.id}</h3>
              <PostForm
                mode="edit"
                initialTitle={editing.title}
                initialContent={editing.content}
                onSubmit={onSaveEdit}
                onCancel={() => setEditing(null)}
              />
            </div>
          ) : null}

          <div style={{ marginTop: 12 }}>
            <PostList posts={posts} role={safeRole} onEdit={(p) => setEditing(p)} onDelete={onDelete} />
          </div>

          <div className="muted" style={{ marginTop: 12 }}>
            Frontend visar UI, backend bestämmer alltid
          </div>
        </div>
      </div>
    </div>
  );
}