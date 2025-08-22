// IssueDesk/frontend/src/components/projects/ProjectsToolbar.tsx
"use client";

import { Input } from "@/components/ui/input";
import { Button } from "@/components/ui/button";
import { Plus, Search } from "lucide-react";

export function ProjectsToolbar({ onCreate }: { onCreate: () => void }) {
  return (
    <div className="flex flex-col gap-3 sm:flex-row sm:items-center sm:justify-between">
      <div className="flex items-center gap-2">
        <Search className="size-4 text-muted-foreground" />
        <Input placeholder="Search projects…" className="w-72" />
      </div>
      <Button onClick={onCreate} className="rounded-2xl">
        <Plus className="mr-2 size-4" /> New Project
      </Button>
    </div>
  );
}
