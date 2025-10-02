import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { ApiService, SubscriberMailingListModel, MailingListModel } from '../../core/api.service';

@Component({
  selector: 'app-subscriber-mailing-lists',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
  <div class="container py-3">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">Subscriber Mailing Lists</h2>
    </div>

    <div *ngIf="loading" class="alert alert-info py-2">Loading...</div>
    <div *ngIf="error" class="alert alert-danger py-2">{{error}}</div>

    <!-- Current subscriptions -->
    <h5 class="mt-2">Current</h5>
    <table class="table table-sm table-striped" *ngIf="!loading && items.length; else noCurrent">
      <thead>
        <tr>
          <th>Subject</th>
          <th>Content</th>
          <th>Last Sent</th>
          <th class="text-end">Actions</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let m of items">
          <td>{{ m.subject }}</td>
          <td class="text-truncate" style="max-width: 420px;">{{ m.content }}</td>
          <td>{{ m.lastSentDate ? (m.lastSentDate | date:'medium') : '-' }}</td>
          <td class="text-end">
            <button class="btn btn-danger btn-sm" (click)="remove(m.id)" [disabled]="loading">Remove</button>
          </td>
        </tr>
      </tbody>
    </table>
    <ng-template #noCurrent>
      <div class="text-muted">No mailing lists yet.</div>
    </ng-template>

    <!-- Available to subscribe -->
    <h5 class="mt-4">Available to subscribe</h5>
    <table class="table table-sm table-hover" *ngIf="!loading && available.length; else noAvailable">
      <thead>
        <tr>
          <th>Subject</th>
          <th>Content</th>
          <th class="text-end">Action</th>
        </tr>
      </thead>
      <tbody>
        <tr *ngFor="let a of available">
          <td>{{ a.subject }}</td>
          <td class="text-truncate" style="max-width: 420px;">{{ a.content }}</td>
          <td class="text-end">
            <button class="btn btn-primary btn-sm" (click)="add(a.id)" [disabled]="loading">Add</button>
          </td>
        </tr>
      </tbody>
    </table>
    <ng-template #noAvailable>
      <div class="text-muted">No available mailing lists.</div>
    </ng-template>
  </div>
  `
})
export class SubscriberMailingListsComponent implements OnInit {
  private readonly api = inject(ApiService);
  private readonly route = inject(ActivatedRoute);

  subscriberId: string = '';
  items: SubscriberMailingListModel[] = [];
  all: MailingListModel[] = [];
  available: MailingListModel[] = [];
  loading = false;
  error = '';

  ngOnInit(): void {
    const id = this.route.snapshot.paramMap.get('id');
    this.subscriberId = id ?? '';
    this.load();
  }

  load(): void {
    if (!this.subscriberId) return;
    this.loading = true;
    this.error = '';
    // Load both current and all, then compute available
    this.api.getSubscriberMailingLists(this.subscriberId).subscribe({
      next: res => {
        this.items = res.items;
        this.api.getMailingLists().subscribe({
          next: allRes => {
            this.all = allRes.items;
            const currentIds = new Set(this.items.map(i => i.id));
            this.available = this.all.filter(a => !currentIds.has(a.id));
            this.loading = false;
          },
          error: err2 => {
            this.error = err2?.error?.message || 'Failed to load all mailing lists';
            this.loading = false;
          }
        });
      },
      error: err => {
        this.error = err?.error?.message || 'Failed to load mailing lists';
        this.loading = false;
      }
    });
  }

  add(mailingListId: string): void {
    if (!this.subscriberId || !mailingListId) return;
    this.loading = true;
    this.error = '';
    this.api.addMailingListToSubscriber(this.subscriberId, mailingListId).subscribe({
      next: () => this.load(),
      error: err => {
        this.error = err?.error?.message || 'Failed to add mailing list';
        this.loading = false;
      }
    });
  }

  remove(mailingListId: string): void {
    if (!confirm('Remove this mailing list from the subscriber?')) return;
    this.loading = true;
    this.error = '';
    this.api.removeMailingListFromSubscriber(this.subscriberId, mailingListId).subscribe({
      next: () => this.load(),
      error: err => {
        this.error = err?.error?.message || 'Failed to remove mailing list';
        this.loading = false;
      }
    });
  }
}
