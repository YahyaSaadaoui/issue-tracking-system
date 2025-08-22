// IssueDesk/frontend/src/components/layout/TopNav.tsx
"use client";

import { Link } from "react-router-dom";
import { Button } from "@/components/ui/button";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";
import { Menu, FolderKanban } from "lucide-react";
import { useState } from "react";

export function TopNav() {
  const [open, setOpen] = useState(false);

  const NavLinks = () => (
    <nav className="flex items-center gap-6">
      <Link to="/" className="text-sm text-muted-foreground hover:text-foreground">
        Projects
      </Link>
    </nav>
  );

  return (
    <header className="sticky top-0 z-30 w-full border-b bg-background/80 backdrop-blur supports-[backdrop-filter]:bg-background/60">
      <div className="container mx-auto flex h-14 items-center justify-between px-4">
        <Link to="/" className="flex items-center gap-2">
          <FolderKanban className="size-5" />
          <span className="font-semibold">IssueDesk</span>
        </Link>

        <div className="hidden md:block">
          <NavLinks />
        </div>

        <div className="md:hidden">
          <Sheet open={open} onOpenChange={setOpen}>
            <SheetTrigger asChild>
              <Button variant="ghost" size="icon" className="rounded-2xl">
                <Menu className="size-5" />
              </Button>
            </SheetTrigger>
            <SheetContent side="right" className="w-72">
              <div className="mt-6 space-y-4">
                <NavLinks />
              </div>
            </SheetContent>
          </Sheet>
        </div>
      </div>
    </header>
  );
}
