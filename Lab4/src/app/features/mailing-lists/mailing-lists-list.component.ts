import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService, MailingListModel } from '../../core/api.service';

@Component({
  selector: 'app-mailing-lists-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
  <div class="container py-3">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">Mailing Lists</h2>
      <button class="btn btn-primary btn-sm" (click)="create()">Create</button>
    </div>

    <div *ngIf="loading" class="alert alert-info py-2">Loading...</div>
    <div *ngIf="error" class="alert alert-danger py-2">{{error}}</div>

    <table class="table table-sm table-striped" *ngIf="!loading && items.length">
      <thead>
        <tr>
          <th>Subject</th>
          <th>Content</th>
          <th class="text-end">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let m of items">
          <td>{{ m.subject }}</td>
          <td class="text-truncate" style="max-width: 520px;">{{ m.content }}</td>
          <td class="text-end">
            <a class="btn btn-outline-secondary btn-sm me-1" [routerLink]="['/mailing-lists', m.id]">View</a>
            <a class="btn btn-secondary btn-sm me-1" [routerLink]="['/mailing-lists', m.id]">Edit</a>
            <button class="btn btn-danger btn-sm" (click)="remove(m.id)" [disabled]="loading">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div *ngIf="!loading && !items.length" class="text-muted">No mailing lists yet.</div>
  </div>
  `
})
export class MailingListsListComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  items: MailingListModel[] = [];
  loading = false;
  error = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.api.getMailingLists().subscribe({
      next: res => {
        this.items = res.items;
        this.loading = false;
      },
      error: err => {
        this.error = err?.error?.message || 'Failed to load mailing lists';
        this.loading = false;
      }
    });
  }

  create(): void {
    this.router.navigate(['/mailing-lists/new']);
  }

  remove(id: string): void {
    if (!confirm('Delete this mailing list?')) return;
    this.loading = true;
    this.api.deleteMailingList(id).subscribe({
      next: () => this.load(),
      error: err => {
        this.error = err?.error?.message || 'Failed to delete mailing list';
        this.loading = false;
      }
    });
  }
}
