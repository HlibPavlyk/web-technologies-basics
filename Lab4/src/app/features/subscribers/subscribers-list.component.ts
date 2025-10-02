import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Router, RouterLink } from '@angular/router';
import { ApiService, SubscriberModel } from '../../core/api.service';

@Component({
  selector: 'app-subscribers-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
  <div class="container py-3">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">Subscribers</h2>
      <button class="btn btn-primary btn-sm" (click)="create()">Create</button>
    </div>

    <div *ngIf="loading" class="alert alert-info py-2">Loading...</div>
    <div *ngIf="error" class="alert alert-danger py-2">{{error}}</div>

    <table class="table table-sm table-striped" *ngIf="!loading && items.length">
      <thead>
        <tr>
          <th>First Name</th>
          <th>Last Name</th>
          <th>Email</th>
          <th class="text-end">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let s of items">
          <td>{{ s.firstName }}</td>
          <td>{{ s.lastName }}</td>
          <td>{{ s.email }}</td>
          <td class="text-end">
            <a class="btn btn-outline-secondary btn-sm me-1" [routerLink]="['/subscribers', s.id]">View</a>
            <a class="btn btn-outline-primary btn-sm me-1" [routerLink]="['/subscribers', s.id, 'mailing-lists']">Mailing Lists</a>
            <a class="btn btn-secondary btn-sm me-1" [routerLink]="['/subscribers', s.id]">Edit</a>
            <button class="btn btn-danger btn-sm" (click)="remove(s.id)" [disabled]="loading">Delete</button>
          </td>
        </tr>
      </tbody>
    </table>

    <div *ngIf="!loading && !items.length" class="text-muted">No subscribers yet.</div>
  </div>
  `
})
export class SubscribersListComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly router = inject(Router);

  items: SubscriberModel[] = [];
  loading = false;
  error = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    this.error = '';
    this.api.getSubscribers().subscribe({
      next: res => {
        this.items = res.items;
        this.loading = false;
      },
      error: err => {
        this.error = err?.error?.message || 'Failed to load subscribers';
        this.loading = false;
      }
    });
  }

  create(): void {
    this.router.navigate(['/subscribers/new']);
  }

  remove(id: string): void {
    if (!confirm('Delete this subscriber?')) return;
    this.loading = true;
    this.api.deleteSubscriber(id).subscribe({
      next: () => this.load(),
      error: err => {
        this.error = err?.error?.message || 'Failed to delete subscriber';
        this.loading = false;
      }
    });
  }
}
