import type { PostDto, Role } from "../types/api";
import { getUserId } from "../auth/authStore";

type Props = {
  post: PostDto;
  role: Role;
  onEdit: (post: PostDto) => void;
  onDelete: (post: PostDto) => void;
};

export function PostItem({ post, role, onEdit, onDelete }: Props) {
  const userId = getUserId();

  const canEdit =
    role === "admin" ||
    (role === "staff" && post.created_By === userId);

  const canDelete = role === "admin";

  return (
    <div className="card">
      <div className="cardTitle">{post.title}</div>
      <div style={{ whiteSpace: "pre-wrap" }}>{post.content}</div>

        <div className="cardMeta">
        Skapad av: {post.createdByEmail} | userId: {post.created_By} | postId: {post.id}
        </div>

      <div className="row" style={{ marginTop: 10 }}>
        {canEdit ? (
          <button className="btn btnSecondary" onClick={() => onEdit(post)}>
            Uppdatera
          </button>
        ) : null}

        {canDelete ? (
          <button className="btn btnDanger" onClick={() => onDelete(post)}>
            Radera
          </button>
        ) : null}
      </div>
    </div>
  );
}