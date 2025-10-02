import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ApiService } from '../../core/api.service';

@Component({
  selector: 'app-subscriber-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
  <div class="container py-3" [formGroup]="form">
    <div class="d-flex justify-content-between align-items-center mb-3">
      <h2 class="m-0">{{ isEdit ? 'Edit Subscriber' : 'Create Subscriber' }}</h2>
      <div>
        <button class="btn btn-secondary btn-sm me-2" type="button" (click)="cancel()">Cancel</button>
        <button class="btn btn-primary btn-sm" type="button" (click)="save()" [disabled]="loading || form.invalid">Save</button>
      </div>
    </div>

    <div *ngIf="loading" class="alert alert-info py-2">Loading...</div>
    <div *ngIf="error" class="alert alert-danger py-2">{{error}}</div>

    <div class="row g-3">
      <div class="col-md-6">
        <label class="form-label">First Name</label>
        <input class="form-control" formControlName="firstName" placeholder="John" />
      </div>
      <div class="col-md-6">
        <label class="form-label">Last Name</label>
        <input class="form-control" formControlName="lastName" placeholder="Doe" />
      </div>

      <div class="col-md-6">
        <label class="form-label">Email</label>
        <input class="form-control" formControlName="email" type="email" placeholder="john@example.com" />
      </div>
      <div class="col-md-6">
        <label class="form-label">Password</label>
        <input class="form-control" formControlName="password" type="password" placeholder="••••••••" />
        <div class="form-text" *ngIf="isEdit">Leave blank to keep current password.</div>
      </div>
    </div>
  </div>
  `
})
export class SubscriberFormComponent implements OnInit {
  private readonly fb = inject(FormBuilder);
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly api = inject(ApiService);

  id: string | null = null;
  isEdit = false;
  loading = false;
  error = '';

  form = this.fb.group({
    firstName: ['', [Validators.required, Validators.maxLength(100)]],
    lastName: ['', [Validators.required, Validators.maxLength(100)]],
    email: ['', [Validators.required, Validators.email, Validators.maxLength(200)]],
    password: ['']
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
    this.api.getSubscriber(this.id).subscribe({
      next: s => {
        this.form.patchValue({
          firstName: s.firstName,
          lastName: s.lastName,
          email: s.email,
          password: ''
        });
        this.loading = false;
      },
      error: err => {
        this.error = err?.error?.message || 'Failed to load subscriber';
        this.loading = false;
      }
    });
  }

  save(): void {
    if (this.form.invalid) return;
    const body = {
      firstName: this.form.value.firstName!,
      lastName: this.form.value.lastName!,
      email: this.form.value.email!,
      password: this.form.value.password || ''
    };

    this.loading = true;
    this.error = '';

    if (this.isEdit && this.id) {
      this.api.updateSubscriber(this.id, body).subscribe({
        next: () => this.router.navigate(['/subscribers']),
        error: err => {
          this.error = err?.error?.message || 'Failed to update subscriber';
          this.loading = false;
        }
      });
    } else {
      this.api.createSubscriber(body).subscribe({
        next: () => this.router.navigate(['/subscribers']),
        error: err => {
          this.error = err?.error?.message || 'Failed to create subscriber';
          this.loading = false;
        }
      });
    }
  }

  cancel(): void {
    this.router.navigate(['/subscribers']);
  }
}
