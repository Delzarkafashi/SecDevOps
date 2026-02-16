import type { PostCreateRequest, PostDto, PostUpdateRequest } from "../types/api";
import { http } from "./http";

export function getPosts() {
  return http<PostDto[]>("/api/posts", "GET");
}

export function createPost(req: PostCreateRequest) {
  return http<PostDto>("/api/posts", "POST", req);
}

export function updatePost(id: number, req: PostUpdateRequest) {
  return http<PostDto>(`/api/posts/${id}`, "PUT", req);
}

export function deletePost(id: number) {
  return http<void>(`/api/posts/${id}`, "DELETE");
}