import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import api from '../services/api';
import type { Course } from '../types';
import { Plus, Search, Filter, BookOpen, Trash2, Edit3, CheckCircle, XCircle, ChevronLeft, ChevronRight, LogOut } from 'lucide-react';
import { motion, AnimatePresence } from 'framer-motion';
import { useAuth } from '../context/AuthContext';

const CoursesPage = () => {
  const [courses, setCourses] = useState<Course[]>([]);
  const [search, setSearch] = useState('');
  const [status, setStatus] = useState<string>('');
  const [page, setPage] = useState(1);
  const [totalCount, setTotalCount] = useState(0);
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [editingCourse, setEditingCourse] = useState<Course | null>(null);
  const [title, setTitle] = useState('');
  const { user, logout } = useAuth();
  const navigate = useNavigate();

  const fetchCourses = async () => {
    try {
      const queryParams: any = { page, pageSize: 6 };
      if (search) queryParams.q = search;
      if (status) queryParams.status = status;
      const response = await api.get('/courses/search', { params: queryParams });
      setCourses(response.data.items);
      setTotalCount(response.data.totalCount);
    } catch (err) {
      console.error(err);
    }
  };

  useEffect(() => { fetchCourses(); }, [search, status, page]);

  const handleSave = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      if (editingCourse) {
        await api.put(`/courses/${editingCourse.id}`, { title });
      } else {
        await api.post('/courses', { title });
      }
      setIsModalOpen(false);
      setEditingCourse(null);
      setTitle('');
      fetchCourses();
    } catch (err: any) { 
      console.error(err); 
      alert(err.response?.data?.message || 'Error al guardar el curso');
    }
  };

  const handleDelete = async (id: string) => {
    if (window.confirm('¿Deseas eliminar este curso?')) {
      await api.delete(`/courses/${id}`);
      fetchCourses();
    }
  };

  const togglePublish = async (course: Course) => {
    try {
      const endpoint = course.status === 'Published' ? 'unpublish' : 'publish';
      await api.patch(`/courses/${course.id}/${endpoint}`);
      fetchCourses();
    } catch (err: any) {
      alert(err.response?.data?.message || 'Error al cambiar estado');
    }
  };

  return (
    <div className="min-h-screen p-8 max-w-7xl mx-auto">
      {/* Header */}
      <div className="flex justify-between items-center mb-12">
        <div>
          <h1 className="text-4xl font-bold mb-2">Hola, {user?.fullName} 👋</h1>
          <p className="text-[var(--text-secondary)]">Gestiona tus cursos y lecciones de forma premium.</p>
        </div>
        <button onClick={logout} className="btn bg-red-500/10 text-red-500 border border-red-500/20 hover:bg-red-500 hover:text-white">
          <LogOut size={20} /> Salir
        </button>
      </div>

      {/* Controls */}
      <div className="flex flex-col md:flex-row gap-4 mb-8">
        <div className="flex-1 relative">
          <Search className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--text-secondary)]" size={20} />
          <input 
            type="text" 
            placeholder="Buscar cursos..." 
            className="pl-12"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
          />
        </div>
        <div className="flex gap-4">
          <div className="relative">
            <Filter className="absolute left-3 top-1/2 -translate-y-1/2 text-[var(--text-secondary)]" size={20} />
            <select 
              className="pl-12 w-48"
              value={status}
              onChange={(e) => setStatus(e.target.value)}
            >
              <option value="">Todos los estados</option>
              <option value="Draft">Borrador</option>
              <option value="Published">Publicado</option>
            </select>
          </div>
          <button onClick={() => { setIsModalOpen(true); setEditingCourse(null); setTitle(''); }} className="btn btn-primary">
            <Plus size={20} /> Crear Curso
          </button>
        </div>
      </div>

      {/* Grid */}
      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
        <AnimatePresence>
          {courses.map((course) => (
            <motion.div 
              key={course.id}
              layout
              initial={{ opacity: 0, scale: 0.9 }}
              animate={{ opacity: 1, scale: 1 }}
              exit={{ opacity: 0, scale: 0.9 }}
              className="premium-card flex flex-col justify-between"
            >
              <div>
                <div className="flex justify-between items-start mb-4">
                  <span className={`badge ${course.status === 'Published' ? 'badge-published' : 'badge-draft'}`}>
                    {course.status === 'Published' ? 'Publicado' : 'Borrador'}
                  </span>
                  <div className="flex gap-2">
                    <button onClick={() => { setEditingCourse(course); setTitle(course.title); setIsModalOpen(true); }} className="text-[var(--text-secondary)] hover:text-white">
                      <Edit3 size={18} />
                    </button>
                    <button onClick={() => handleDelete(course.id)} className="text-[var(--text-secondary)] hover:text-red-500">
                      <Trash2 size={18} />
                    </button>
                  </div>
                </div>
                <h3 className="text-xl mb-6 leading-tight">{course.title}</h3>
              </div>
              <div className="flex flex-col gap-3">
                <button 
                  onClick={() => navigate(`/courses/${course.id}/lessons`)}
                  className="btn bg-[var(--accent-primary)]/10 text-[var(--accent-primary)] border border-[var(--accent-primary)]/20 hover:bg-[var(--accent-primary)] hover:text-white w-full justify-center"
                >
                  <BookOpen size={18} /> Ver Lecciones
                </button>
                <button 
                  onClick={() => togglePublish(course)}
                  className={`btn border w-full justify-center ${course.status === 'Published' ? 'border-red-500/20 text-red-500 hover:bg-red-500 hover:text-white' : 'border-emerald-500/20 text-emerald-500 hover:bg-emerald-500 hover:text-white'}`}
                >
                  {course.status === 'Published' ? <XCircle size={18} /> : <CheckCircle size={18} />}
                  {course.status === 'Published' ? 'Despublicar' : 'Publicar'}
                </button>
              </div>
            </motion.div>
          ))}
        </AnimatePresence>
      </div>

      {/* Pagination */}
      <div className="flex justify-center items-center gap-4 mt-12 pb-12">
        <button disabled={page === 1} onClick={() => setPage(p => p - 1)} className="btn bg-white/5 border border-white/10 disabled:opacity-30">
          <ChevronLeft size={20} />
        </button>
        <span className="text-sm">Página {page} de {Math.ceil(totalCount / 6) || 1}</span>
        <button disabled={page >= Math.ceil(totalCount / 6)} onClick={() => setPage(p => p + 1)} className="btn bg-white/5 border border-white/10 disabled:opacity-30">
          <ChevronRight size={20} />
        </button>
      </div>

      {/* Modal Creating/Editing */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black/80 backdrop-blur-sm z-50 flex items-center justify-center p-4">
          <motion.div initial={{ scale: 0.9, opacity: 0 }} animate={{ scale: 1, opacity: 1 }} className="premium-card w-full max-w-lg bg-[#1a1c23]">
            <h2 className="text-2xl mb-6">{editingCourse ? 'Editar Curso' : 'Crear Nuevo Curso'}</h2>
            <form onSubmit={handleSave}>
              <div className="input-group">
                <label>Título del Curso</label>
                <input 
                  type="text" 
                  value={title} 
                  onChange={(e) => setTitle(e.target.value)} 
                  placeholder="Ej. Dominando Clean Architecture"
                  required 
                />
              </div>
              <div className="flex justify-end gap-4 mt-8">
                <button type="button" onClick={() => setIsModalOpen(false)} className="btn text-[var(--text-secondary)]">Cancelar</button>
                <button type="submit" className="btn btn-primary">Guardar Cambios</button>
              </div>
            </form>
          </motion.div>
        </div>
      )}
    </div>
  );
};

export default CoursesPage;
