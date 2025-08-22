// IssueDesk/frontend/src/pages/TicketDetails.tsx
"use client";

import { useParams, Link } from "react-router-dom";
import { useState } from "react";
import { useTicket } from "@/api/queries";
import { useAssignTicket, useChangeTicketStatus, useAddComment } from "@/api/mutations";
import { ChangeStatusBody, AddCommentBody, AssignBody } from "@/api/schemas";

import { Tabs, TabsContent, TabsList, TabsTrigger } from "@/components/ui/tabs";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Textarea } from "@/components/ui/textarea";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Skeleton } from "@/components/ui/skeleton";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { ArrowLeft } from "lucide-react";

const STATUS = ["New", "InProgress", "Resolved", "Closed"] as const;

export default function TicketDetails() {
  const { ticketId = "" } = useParams();
  const { data, isLoading, isError, error } = useTicket(ticketId);
  const [assignee, setAssignee] = useState("");
  const [nextStatus, setNextStatus] = useState<(typeof STATUS)[number]>("InProgress");
  const [author, setAuthor] = useState("agent");
  const [body, setBody] = useState("");

  const projectId = data?.ticket.projectId;
  const assignMutation = useAssignTicket(ticketId, projectId);
  const statusMutation = useChangeTicketStatus(ticketId, projectId);
  const commentMutation = useAddComment(ticketId);

  if (isLoading) {
    return (
      <div className="space-y-4">
        <Skeleton className="h-8 w-56" />
        <Skeleton className="h-5 w-64" />
      </div>
    );
  }

  if (isError) {
    return (
      <Alert variant="destructive" className="rounded-2xl">
        <AlertTitle>Failed to load ticket</AlertTitle>
        <AlertDescription>{(error as Error)?.message}</AlertDescription>
      </Alert>
    );
  }

  if (!data) return null;

  const t = data.ticket;

  return (
    <section className="space-y-6">
      <Button asChild variant="ghost" size="sm" className="rounded-2xl">
        <Link to={`/projects/${t.projectId}`}>
          <ArrowLeft className="mr-2 size-4" />
          Back to project
        </Link>
      </Button>

      <div className="space-y-1">
        <h2 className="text-2xl font-semibold">{t.title}</h2>
        <p className="text-sm text-muted-foreground">
          Status <b>{t.status}</b> • Priority <b>{t.priority}</b> • Assignee{" "}
          <b>{t.assignee ?? "-"}</b>
        </p>
      </div>
      {t.description && <p className="whitespace-pre-wrap text-sm">{t.description}</p>}

      <Tabs defaultValue="overview" className="space-y-4">
        <TabsList className="rounded-2xl">
          <TabsTrigger value="overview">Overview</TabsTrigger>
          <TabsTrigger value="activity">Activity</TabsTrigger>
          <TabsTrigger value="settings">Settings</TabsTrigger>
        </TabsList>

        <TabsContent value="overview">
          <div className="grid grid-cols-1 gap-4 md:grid-cols-2">
            <Card className="rounded-2xl border shadow-sm">
              <CardHeader>
                <CardTitle className="text-base">Assign</CardTitle>
              </CardHeader>
              <CardContent className="flex items-center gap-2">
                <Input
                  value={assignee}
                  onChange={(e) => setAssignee(e.target.value)}
                  placeholder="someone@desk.io"
                />
                <Button
                  className="rounded-2xl"
                  disabled={assignMutation.isPending || !assignee.trim()}
                  onClick={async () => {
                    const payload: AssignBody = { assignee: assignee.trim() };
                    await assignMutation.mutateAsync(payload);
                    setAssignee("");
                  }}
                >
                  {assignMutation.isPending ? "Assigning…" : "Assign"}
                </Button>
              </CardContent>
            </Card>

            <Card className="rounded-2xl border shadow-sm">
              <CardHeader>
                <CardTitle className="text-base">Change status</CardTitle>
              </CardHeader>
              <CardContent className="flex items-center gap-2">
                <Select value={nextStatus} onValueChange={(v) => setNextStatus(v as typeof nextStatus)}>
                  <SelectTrigger className="w-[200px] rounded-2xl">
                    <SelectValue placeholder="Next status" />
                  </SelectTrigger>
                  <SelectContent>
                    {STATUS.map((s) => (
                      <SelectItem key={s} value={s}>
                        {s}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
                <Button
                  className="rounded-2xl"
                  disabled={statusMutation.isPending}
                  onClick={async () => {
                    const payload: ChangeStatusBody = { nextStatus };
                    await statusMutation.mutateAsync(payload);
                  }}
                >
                  {statusMutation.isPending ? "Updating…" : "Update"}
                </Button>
              </CardContent>
            </Card>
          </div>
        </TabsContent>

        <TabsContent value="activity">
          <Card className="rounded-2xl border shadow-sm">
            <CardHeader>
              <CardTitle className="text-base">Add comment</CardTitle>
            </CardHeader>
            <CardContent className="space-y-3">
              <div className="flex items-center gap-2">
                <Input
                  value={author}
                  onChange={(e) => setAuthor(e.target.value)}
                  placeholder="author"
                  className="max-w-xs"
                />
              </div>
              <Textarea
                value={body}
                onChange={(e) => setBody(e.target.value)}
                placeholder="Write a comment…"
                rows={4}
              />
              <Button
                className="rounded-2xl"
                disabled={commentMutation.isPending || !author.trim() || body.trim().length < 3}
                onClick={async () => {
                  const payload: AddCommentBody = { author: author.trim(), body: body.trim() };
                  await commentMutation.mutateAsync(payload);
                  setBody("");
                }}
              >
                {commentMutation.isPending ? "Posting…" : "Post"}
              </Button>
            </CardContent>
          </Card>

          <Card className="mt-4 rounded-2xl border shadow-sm">
            <CardHeader>
              <CardTitle className="text-base">Comments</CardTitle>
            </CardHeader>
            <CardContent>
              {data.comments.length === 0 ? (
                <p className="text-sm text-muted-foreground">No comments yet.</p>
              ) : (
                <ul className="space-y-4">
                  {data.comments.map((c) => (
                    <li key={c.id} className="border-t pt-4 first:border-0 first:pt-0">
                      <div className="font-medium">{c.author}</div>
                      <div className="whitespace-pre-wrap text-sm">{c.body}</div>
                      <div className="text-xs text-muted-foreground">
                        {new Date(c.createdAt).toLocaleString()}
                      </div>
                    </li>
                  ))}
                </ul>
              )}
            </CardContent>
          </Card>
        </TabsContent>

        <TabsContent value="settings">
          <Card className="rounded-2xl border shadow-sm">
            <CardHeader>
              <CardTitle className="text-base">Settings</CardTitle>
            </CardHeader>
            <CardContent className="text-sm text-muted-foreground">
              Ticket-level settings can go here.
            </CardContent>
          </Card>
        </TabsContent>
      </Tabs>
    </section>
  );
}
