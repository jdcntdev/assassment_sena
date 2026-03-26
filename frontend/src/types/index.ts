export interface Course {
  id: string;
  title: string;
  status: 'Draft' | 'Published';
  createdAt: string;
  updatedAt: string;
}

export interface Lesson {
  id: string;
  courseId: string;
  title: string;
  order: number;
}

export interface CourseSummary {
  id: string;
  title: string;
  totalLessons: number;
  updatedAt: string;
}

export interface AuthResponse {
  token: string;
  email: string;
  fullName: string;
}
