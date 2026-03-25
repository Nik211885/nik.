import {ApplicationInitStatus, Injectable} from '@angular/core';
import { HttpClient, HttpEvent, HttpEventType, HttpRequest } from '@angular/common/http';
import {
  EMPTY,
  Observable,
  catchError,
  filter,
  map,
  switchMap,
  throwError,
  forkJoin,
} from 'rxjs';
import {ApplicationMessage} from '../../app.message';
import {LanguageService} from './language.service';

/**
 * Cloudinary upload response model
 */
export interface CloudinaryUploadResponse {

  /**
   * Public URL of uploaded file
   */
  url: string;

  /**
   * Unique identifier in Cloudinary
   */
  publicId: string;

  /**
   * File format (e.g., jpg, png, mp4)
   */
  format: string;

  /**
   * File width (for images/videos)
   */
  width: number;

  /**
   * File height (for images/videos)
   */
  height: number;

  /**
   * File size in bytes
   */
  bytes: number;

  /**
   * Upload timestamp
   */
  createdAt: string;
}

/**
 * Upload progress model
 */
export interface UploadProgress {

  /**
   * Upload progress percentage (0 - 100)
   */
  progress: number;

  /**
   * Final response when upload completes
   */
  response?: CloudinaryUploadResponse;
}

/**
 * File validation rules
 */
const MAX_FILE_SIZE_MB = 10;
const ALLOWED_TYPES = ['image/jpeg', 'image/png', 'image/webp', 'video/mp4'];

/**
 * API endpoints for Cloudinary upload
 */
const ApiUpload = {
  GET_SIGNED_URL: 'api/extend-service/cloudinary/upload-file-by-signature',
} as const;

/**
 * FileCloudinaryService
 *
 * Handles uploading files to Cloudinary using signed URLs.
 *
 * Features:
 * - File validation (type & size)
 * - Signed URL retrieval from backend
 * - Upload with progress tracking
 * - Multiple file upload with aggregation (forkJoin)
 */
@Injectable({ providedIn: 'root' })
export class FileCloudinaryService {

  constructor(private readonly http: HttpClient,
              private readonly languageService: LanguageService) {}

  /**
   * Upload a single file
   *
   * Flow:
   * 1. Validate file
   * 2. Request signed URL
   * 3. Upload file to Cloudinary
   * 4. Emit progress updates
   *
   * @param file - File to upload
   * @returns Observable emitting UploadProgress
   */
  public uploadFile(file: File): Observable<UploadProgress> {

    const validationError = this.validate(file);
    if (validationError) {
      return throwError(() => new Error(validationError));
    }

    return this.getSignedUploadUrl().pipe(
      switchMap((uploadUrl) =>
        this.uploadToCloudinary(file, uploadUrl)
      ),
      catchError((err) => {
        return throwError(() => err);
      })
    );
  }

  /**
   * Upload multiple files in parallel
   *
   * Behavior:
   * - Each file is uploaded independently
   * - Only emits when ALL uploads complete
   * - Returns array of successful upload responses
   *
   * Notes:
   * - Uses forkJoin → waits for all observables to complete
   * - Progress tracking is NOT aggregated here (only final result)
   *
   * @param files - Array of files to upload
   * @returns Observable emitting array of CloudinaryUploadResponse
   *
   * @example
   * service.uploadMultiple(files).subscribe(responses => {
   *   console.log('All uploads done:', responses);
   * });
   */
  public uploadMultiple(files: File[]): Observable<CloudinaryUploadResponse[]> {

    /**
     * Return EMPTY observable if no files provided
     */
    if (!files.length) return EMPTY;

    /**
     * Create upload observables for each file
     * - Filter only completed uploads (progress === 100)
     * - Extract final response
     */
    const uploads$ = files.map((file) =>
      this.uploadFile(file).pipe(
        filter(({ progress }) => progress === 100),
        map(({ response }) => response!)
      )
    );

    /**
     * Execute all uploads in parallel
     * and wait for all to complete
     */
    return forkJoin(uploads$);
  }

  /**
   * Get signed upload URL from backend
   *
   * @private
   * @returns Observable<string>
   */
  private getSignedUploadUrl(): Observable<string> {
    return this.http.get<string>(ApiUpload.GET_SIGNED_URL);
  }

  /**
   * Upload file to Cloudinary using signed URL
   *
   * @private
   * @param file - File to upload
   * @param uploadUrl - Signed URL
   * @returns Observable emitting upload progress and response
   */
  private uploadToCloudinary(
    file: File,
    uploadUrl: string
  ): Observable<UploadProgress> {

    const formData = new FormData();
    formData.append('file', file);

    const request = new HttpRequest('POST', uploadUrl, formData, {
      reportProgress: true,
    });

    return this.http.request<CloudinaryUploadResponse>(request).pipe(

      /**
       * Only process relevant HTTP events
       */
      filter((event) =>
        event.type === HttpEventType.UploadProgress ||
        event.type === HttpEventType.Response
      ),

      /**
       * Map events to UploadProgress model
       */
      map((event: HttpEvent<CloudinaryUploadResponse>) => {

        /**
         * Progress update
         */
        if (event.type === HttpEventType.UploadProgress) {
          const progress = event.total
            ? Math.round((event.loaded / event.total) * 100)
            : 0;

          return { progress };
        }

        /**
         * Upload completed
         */
        if (event.type === HttpEventType.Response && event.body) {
          return { progress: 100, response: event.body };
        }

        return { progress: 0 };
      })
    );
  }

  /**
   * Validate file before upload
   *
   * Rules:
   * - File must exist
   * - File type must be allowed
   * - File size must not exceed MAX_FILE_SIZE_MB
   *
   * @private
   * @param file - File to validate
   * @returns Error message or null if valid
   */
  private validate(file: File): string | null {

    if (!file) {
      return this.languageService.translate(ApplicationMessage.FILE_NOT_EXITS);
    }

    if (!ALLOWED_TYPES.includes(file.type)) {
      return `${this.languageService.translate(ApplicationMessage.FILE_TYPE_APPLY)} ${ALLOWED_TYPES.join(', ')}`;
    }

    const sizeMB = file.size / (1024 * 1024);
    if (sizeMB > MAX_FILE_SIZE_MB) {
      return `${this.languageService.translate(ApplicationMessage.FILE_MAX_SIZE)} ${MAX_FILE_SIZE_MB} MB`;
    }

    return null;
  }
}
