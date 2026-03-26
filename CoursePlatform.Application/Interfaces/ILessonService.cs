using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoursePlatform.Application.DTOs;

namespace CoursePlatform.Application.Interfaces;

public interface ILessonService
{
    Task<LessonDto> CreateLesson(CreateLessonRequest request);
    Task<LessonDto> UpdateLesson(Guid id, UpdateLessonRequest request);
    Task DeleteLesson(Guid id);
    Task<IEnumerable<LessonDto>> GetLessonsByCourse(Guid courseId);
}
