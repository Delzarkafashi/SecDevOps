import type { PostDto, Role } from "../types/api";
import { PostItem } from "./PostItem";

type Props = {
  posts: PostDto[];
  role: Role;
  onEdit: (post: PostDto) => void;
  onDelete: (post: PostDto) => void;
};

export function PostList({ posts, role, onEdit, onDelete }: Props) {
  if (!posts.length) return <div>Inga posts</div>;

  return (
    <div>
      {posts.map((p) => (
        <PostItem key={p.id} post={p} role={role} onEdit={onEdit} onDelete={onDelete} />
      ))}
    </div>
  );
}