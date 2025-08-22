// IssueDesk/frontend/src/pages/ProjectTickets.tsx
"use client";

import { useParams, Link, useNavigate } from "react-router-dom";
import { useMemo, useState } from "react";
import { useProjectTickets } from "@/api/queries";
import { useCreateTicket } from "@/api/mutations";
import { CreateTicketInput } from "@/api/schemas";
import { STATUS_OPTIONS, PRIORITY_OPTIONS } from "@/lib/options";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Dialog, DialogContent, DialogHeader, DialogTitle, DialogTrigger, DialogFooter } from "@/components/ui/dialog";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Select, SelectContent, SelectItem, SelectTrigger, SelectValue } from "@/components/ui/select";
import { Textarea } from "@/components/ui/textarea";
import { Skeleton } from "@/components/ui/skeleton";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { Plus, ArrowLeft } from "lucide-react";

const createSchema = z.object({
  title: z.string().min(5, "Title must be at least 5 characters"),
  description: z.string().optional(),
  priority: z.enum(["Low", "Medium", "High"]),
});

type CreateForm = z.infer<typeof createSchema>;

export default function ProjectTickets() {
  const { projectId = "" } = useParams();
  const navigate = useNavigate();

  const [status, setStatus] = useState<(typeof STATUS_OPTIONS)[number]["value"]>("all");
  const [priority, setPriority] = useState<(typeof PRIORITY_OPTIONS)[number]["value"]>("all");
  const [search, setSearch] = useState("");
  const [page, setPage] = useState(1);
  const pageSize = 10;

  const filters = useMemo(
    () => ({
      status: status === "all" ? undefined : status,
      priority: priority === "all" ? undefined : priority,
      search: search || undefined,
      page,
      pageSize,
    }),
    [status, priority, search, page]
  );

  const { data, isLoading, isError, error, refetch } = useProjectTickets(projectId, filters);
  const createMutation = useCreateTicket(projectId);

  const form = useForm<CreateForm>({
    resolver: zodResolver(createSchema),
    defaultValues: { title: "", description: "", priority: "Medium" },
  });

  async function onCreate(values: CreateForm) {
    const payload: CreateTicketInput = {
      projectId,
      title: values.title.trim(),
      description: values.description?.trim() || undefined,
      priority: values.priority,
    };
    const created = await createMutation.mutateAsync(payload);
    navigate(`/tickets/${created.id}`);
  }

  return (
    <section className="space-y-6">
      <div className="flex items-center gap-3">
        <Button asChild variant="ghost" size="sm" className="rounded-2xl">
          <Link to="/">
            <ArrowLeft className="mr-2 size-4" />
            Projects
          </Link>
        </Button>
        <h2 className="text-xl font-semibold">Tickets</h2>
      </div>

      <Card className="rounded-2xl border shadow-sm">
        <CardContent className="flex flex-col gap-3 p-4 sm:flex-row sm:items-center sm:justify-between">
          <div className="flex flex-wrap items-center gap-3">
            <Select
              value={status}
              onValueChange={(v) => setStatus(v as (typeof STATUS_OPTIONS)[number]["value"])}
            >
              <SelectTrigger className="w-[180px] rounded-2xl">
                <SelectValue placeholder="Status" />
              </SelectTrigger>
              <SelectContent>
                {STATUS_OPTIONS.map((o) => (
                  <SelectItem key={o.value} value={o.value}>
                    {o.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Select
              value={priority}
              onValueChange={(v) => setPriority(v as (typeof PRIORITY_OPTIONS)[number]["value"])}
            >
              <SelectTrigger className="w-[180px] rounded-2xl">
                <SelectValue placeholder="Priority" />
              </SelectTrigger>
              <SelectContent>
                {PRIORITY_OPTIONS.map((o) => (
                  <SelectItem key={o.value} value={o.value}>
                    {o.label}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Input
              value={search}
              onChange={(e) => {
                setPage(1);
                setSearch(e.target.value);
              }}
              placeholder="Search title/description…"
              className="w-64"
            />
          </div>

          <Dialog>
            <DialogTrigger asChild>
              <Button className="rounded-2xl">
                <Plus className="mr-2 size-4" /> New Ticket
              </Button>
            </DialogTrigger>
            <DialogContent className="rounded-2xl sm:max-w-md">
              <DialogHeader>
                <DialogTitle>Create ticket</DialogTitle>
              </DialogHeader>

              <form
                className="space-y-3"
                onSubmit={form.handleSubmit(onCreate)}
                id="create-ticket-form"
              >
                <Input
                  placeholder="Title (5–120 chars)"
                  {...form.register("title")}
                />
                {form.formState.errors.title && (
                  <p className="text-sm text-destructive">{form.formState.errors.title.message}</p>
                )}
                <Textarea
                  placeholder="Description (optional)"
                  rows={4}
                  {...form.register("description")}
                />
                <Select
                  value={form.watch("priority")}
                  onValueChange={(v) => form.setValue("priority", v as CreateForm["priority"])}
                >
                  <SelectTrigger className="rounded-2xl">
                    <SelectValue placeholder="Priority" />
                  </SelectTrigger>
                  <SelectContent>
                    {(["Low", "Medium", "High"] as const).map((p) => (
                      <SelectItem key={p} value={p}>
                        {p}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>
              </form>

              <DialogFooter>
                <Button
                  type="submit"
                  form="create-ticket-form"
                  className="rounded-2xl"
                  disabled={createMutation.isPending}
                >
                  {createMutation.isPending ? "Creating…" : "Create"}
                </Button>
              </DialogFooter>
            </DialogContent>
          </Dialog>
        </CardContent>
      </Card>

      {isError && (
        <Alert variant="destructive" className="rounded-2xl">
          <AlertTitle>Could not load tickets</AlertTitle>
          <AlertDescription>{(error as Error)?.message}</AlertDescription>
        </Alert>
      )}

      <Card className="rounded-2xl border shadow-sm">
        <CardHeader>
          <CardTitle className="text-base">Results</CardTitle>
        </CardHeader>
        <CardContent>
          {isLoading ? (
            <div className="space-y-2">
              {Array.from({ length: 6 }).map((_, i) => (
                <Skeleton key={i} className="h-10 w-full" />
              ))}
            </div>
          ) : (
            <>
              <Table>
                <TableHeader>
                  <TableRow>
                    <TableHead>Title</TableHead>
                    <TableHead>Status</TableHead>
                    <TableHead>Priority</TableHead>
                    <TableHead>Assignee</TableHead>
                    <TableHead>Updated</TableHead>
                  </TableRow>
                </TableHeader>
                <TableBody>
                  {data?.items.map((t) => (
                    <TableRow key={t.id}>
                      <TableCell className="font-medium">
                        <Link to={`/tickets/${t.id}`} className="hover:underline">
                          {t.title}
                        </Link>
                      </TableCell>
                      <TableCell>{t.status}</TableCell>
                      <TableCell>{t.priority}</TableCell>
                      <TableCell>{t.assignee ?? "-"}</TableCell>
                      <TableCell>{new Date(t.updatedAt).toLocaleString()}</TableCell>
                    </TableRow>
                  ))}
                  {data && data.items.length === 0 && (
                    <TableRow>
                      <TableCell colSpan={5} className="text-muted-foreground">
                        No tickets match your filters.
                      </TableCell>
                    </TableRow>
                  )}
                </TableBody>
              </Table>

              {data && (
                <div className="mt-4 flex items-center justify-between">
                  <div className="text-sm text-muted-foreground">
                    {data.total} total • page {data.page} of{" "}
                    {Math.max(1, Math.ceil(data.total / data.pageSize))}
                  </div>
                  <div className="flex gap-2">
                    <Button
                      variant="secondary"
                      className="rounded-2xl"
                      onClick={() => {
                        setPage((p) => Math.max(1, p - 1));
                        refetch();
                      }}
                      disabled={data.page === 1}
                    >
                      Prev
                    </Button>
                    <Button
                      className="rounded-2xl"
                      onClick={() => {
                        setPage((p) => p + 1);
                        refetch();
                      }}
                      disabled={data.page * data.pageSize >= data.total}
                    >
                      Next
                    </Button>
                  </div>
                </div>
              )}
            </>
          )}
        </CardContent>
      </Card>
    </section>
  );
}
