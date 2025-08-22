// IssueDesk/frontend/src/components/projects/EmptyProjects.tsx
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { FolderPlus } from "lucide-react";

export function EmptyProjects({ onCreate }: { onCreate: () => void }) {
  return (
    <Card className="rounded-2xl border shadow-sm">
      <CardContent className="flex flex-col items-center gap-4 py-16 text-center">
        <FolderPlus className="size-8 text-muted-foreground" />
        <div className="space-y-1">
          <h3 className="text-xl font-semibold">No projects yet</h3>
          <p className="text-sm text-muted-foreground">
            Create your first project to get started.
          </p>
        </div>
        <Button onClick={onCreate} className="rounded-2xl">
          Create project
        </Button>
      </CardContent>
    </Card>
  );
}
