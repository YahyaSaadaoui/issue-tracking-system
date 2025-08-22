// IssueDesk/frontend/src/App.tsx
import { Outlet } from "react-router-dom";
import { TopNav } from "@/components/layout/TopNav";

export default function App() {
  return (
    <div className="min-h-dvh bg-background text-foreground">
      <TopNav />
      <main className="container mx-auto px-4 py-6">
        <Outlet />
      </main>
    </div>
  );
}
