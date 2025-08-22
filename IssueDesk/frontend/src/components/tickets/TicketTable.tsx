import { Link } from "react-router-dom";
import { Badge } from "@/components/ui/badge";
import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table";

type Row = {
  id: string;
  title: string;
  status: "New" | "InProgress" | "Resolved" | "Closed";
  priority: "Low" | "Medium" | "High";
  assignee?: string | null;
  updatedAt: string;
};

export function TicketTable({ rows }: { rows: Row[] }) {
  return (
    <div className="overflow-hidden rounded-2xl border shadow-sm">
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
          {rows.map((t) => (
            <TableRow key={t.id}>
              <TableCell className="font-medium">
                <Link
                  to={`/tickets/${t.id}`}
                  className="underline decoration-dotted underline-offset-4"
                >
                  {t.title}
                </Link>
              </TableCell>
              <TableCell>
                <Badge variant="secondary" className="rounded-2xl">
                  {t.status}
                </Badge>
              </TableCell>
              <TableCell>
                <Badge className="rounded-2xl">{t.priority}</Badge>
              </TableCell>
              <TableCell className="text-muted-foreground">
                {t.assignee ?? "-"}
              </TableCell>
              <TableCell className="text-muted-foreground">
                {new Date(t.updatedAt).toLocaleString()}
              </TableCell>
            </TableRow>
          ))}
          {rows.length === 0 && (
            <TableRow>
              <TableCell colSpan={5} className="text-center text-muted-foreground">
                No tickets.
              </TableCell>
            </TableRow>
          )}
        </TableBody>
      </Table>
    </div>
  );
}
