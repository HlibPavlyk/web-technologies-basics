import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-mailing-list-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
  <div class="container py-3" [formGroup]="form">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">{{ isEdit ? 'Edit Mailing List' : 'Create Mailing List' }}</h2>
      <div>
        <button class="btn btn-secondary btn-sm me-2" type="button" (click)="cancel()">Cancel</button>
        <button class="btn btn-primary btn-sm" type="button" (click)="save()" [disabled]="loading || form.invalid">Save</button>
      </div>
    </div>

    <div *ngIf="loading" class="alert alert-info py-2">Loading...</div>
    <div *ngIf="error" class="alert alert-danger py-2">{{error}}</div>

    <div class="mb-3">
      <label class="form-label">Subject</label>
      <input class="form-control" formControlName="subject" placeholder="Subject" />
    </div>
    <div class="mb-3">
      <label class="form-label">Content</label>
      <textarea rows="6" class="form-control" formControlName="content" placeholder="Email content..."></textarea>
    </div>
  </div>
  `
})
export class MailingListFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ApiService);

  id: string | null = null;
  isEdit = false;
  loading = false;
  error = '';

  form = this.fb.group({
    subject: ['', [Validators.required, Validators.maxLength(200)]],
    content: ['', [Validators.required]]
  });

  ngOnInit(): void {
    this.id = this.route.snapshot.paramMap.get('id');
    this.isEdit = !!this.id;
    if (this.isEdit && this.id) {
      this.fetch();
    }
  }

  fetch(): void {
    if (!this.id) return;
    this.loading = true;
    this.error = '';
    this.api.getMailingList(this.id).subscribe({
      next: m => {
        this.form.patchValue({
          subject: m.subject,
          content: m.content,
        });
        this.loading = false;
      },
      error: err => {
        this.error = err?.error?.message || 'Failed to load mailing list';
        this.loading = false;
      }
    });
  }

  save(): void {
    if (this.form.invalid) return;
    const body = {
      subject: this.form.value.subject!,
      content: this.form.value.content!,
    };

    this.loading = true;
    this.error = '';

    if (this.isEdit && this.id) {
      this.api.updateMailingList(this.id, body).subscribe({
        next: () => this.router.navigate(['/mailing-lists']),
        error: err => {
          this.error = err?.error?.message || 'Failed to update mailing list';
          this.loading = false;
        }
      });
    } else {
      this.api.createMailingList(body).subscribe({
        next: () => this.router.navigate(['/mailing-lists']),
        error: err => {
          this.error = err?.error?.message || 'Failed to create mailing list';
          this.loading = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/mailing-lists']);
  }
}
