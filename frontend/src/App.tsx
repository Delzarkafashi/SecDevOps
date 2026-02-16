import { useAuth } from "./auth/useAuth";
import { LoginPage } from "./pages/LoginPage";
import { PostsPage } from "./pages/PostsPage";
import "./App.css";

export default function App() {
  const { isAuthed } = useAuth();
  return isAuthed ? <PostsPage /> : <LoginPage />;
}