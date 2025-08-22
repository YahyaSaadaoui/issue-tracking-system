"use client";

import * as React from "react";
import { Link } from "react-router-dom";
import { useProjects } from "@/api/queries";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { FolderPlus, FolderOpen } from "lucide-react";
import { Button } from "@/components/ui/button";
import { ProjectsToolbar } from "@/components/projects/ProjectsToolbar";
import { CreateProjectDialog } from "@/components/projects/CreateProjectDialog";

export default function Projects() {
  const { data, isLoading, isError, error } = useProjects();
  const [open, setOpen] = React.useState(false);

  return (
    <section className="container mx-auto max-w-6xl space-y-6 p-4 sm:p-6">
      <header className="space-y-4">
        <div className="flex items-center justify-between">
          <h2 className="text-2xl font-semibold tracking-tight">Projects</h2>
        </div>
        <ProjectsToolbar onCreate={() => setOpen(true)} />
      </header>

      {isLoading ? (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {Array.from({ length: 6 }).map((_, i) => (
            <Card key={i} className="rounded-2xl border shadow-sm">
              <CardHeader>
                <Skeleton className="h-6 w-40" />
              </CardHeader>
              <CardContent className="space-y-3">
                <Skeleton className="h-4 w-48" />
                <Skeleton className="h-8 w-24" />
              </CardContent>
            </Card>
          ))}
        </div>
      ) : isError ? (
        <Alert variant="destructive" className="rounded-2xl">
          <AlertTitle>Couldn’t load projects</AlertTitle>
          <AlertDescription>{(error as Error)?.message}</AlertDescription>
        </Alert>
      ) : !data || data.length === 0 ? (
        <Card className="rounded-2xl border shadow-sm">
          <CardContent className="flex flex-col items-center gap-4 py-16 text-center">
            <FolderPlus className="size-8 text-muted-foreground" />
            <div>
              <h3 className="text-xl font-semibold">No projects yet</h3>
              <p className="text-sm text-muted-foreground">
                Create your first project to get started.
              </p>
            </div>
            <Button className="rounded-2xl" onClick={() => setOpen(true)}>
              New Project
            </Button>
          </CardContent>
        </Card>
      ) : (
        <div className="grid grid-cols-1 gap-4 sm:grid-cols-2 lg:grid-cols-3">
          {data.map((p) => (
            <Card key={p.id} className="group rounded-2xl border shadow-sm">
              <CardHeader>
                <CardTitle className="flex items-center gap-2">
                  <FolderOpen className="size-5 text-muted-foreground" />
                  <span>{p.name}</span>
                  <span className="text-sm text-muted-foreground">[{p.key}]</span>
                </CardTitle>
              </CardHeader>
              <CardContent className="space-y-2">
                <p className="text-sm text-muted-foreground">
                  Created {new Date(p.createdAt).toLocaleDateString()}
                </p>
                <Button asChild variant="secondary" className="rounded-2xl">
                  <Link to={`/projects/${p.id}`}>View tickets →</Link>
                </Button>
              </CardContent>
            </Card>
          ))}
        </div>
      )}

      <CreateProjectDialog open={open} onOpenChange={setOpen} />
    </section>
  );
}
