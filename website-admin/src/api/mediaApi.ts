import axiosClient from './axiosClient';
import type { MediaFileResponse } from '../types/responses';

export const mediaApi = {
  uploadImage: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return axiosClient.post<never, MediaFileResponse>('/api/v1/admin/media/upload-image', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
  uploadAudio: (file: File) => {
    const formData = new FormData();
    formData.append('file', file);
    return axiosClient.post<never, MediaFileResponse>('/api/v1/admin/media/upload-audio', formData, {
      headers: { 'Content-Type': 'multipart/form-data' },
    });
  },
};
