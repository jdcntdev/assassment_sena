import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAuth } from '../context/AuthContext';
import api from '../services/api';
import { LogIn, UserPlus } from 'lucide-react';
import { motion } from 'framer-motion';

const LoginPage = () => {
  const [isLogin, setIsLogin] = useState(true);
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [fullName, setFullName] = useState('');
  const [error, setError] = useState('');
  const { login } = useAuth();
  const navigate = useNavigate();

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    try {
      const endpoint = isLogin ? '/auth/login' : '/auth/register';
      const payload = isLogin ? { email, password } : { email, password, fullName };
      const response = await api.post(endpoint, payload);
      login(response.data);
      navigate('/courses');
    } catch (err: any) {
      setError(err.response?.data?.message || 'Algo salió mal');
    }
  };

  return (
    <div className="min-h-screen flex items-center justify-center p-4">
      <motion.div 
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        className="premium-card w-full max-w-md"
      >
        <div className="flex justify-center mb-8">
          <div className="w-16 h-16 rounded-2xl bg-gradient-to-br from-[var(--accent-primary)] to-[var(--accent-secondary)] flex items-center justify-center text-white">
            <LogIn size={32} />
          </div>
        </div>
        <h1 className="text-3xl font-bold text-center mb-2 tracking-tight">Bienvenido</h1>
        <p className="text-center text-[var(--text-secondary)] mb-8">
          {isLogin ? 'Ingresa a tu plataforma de aprendizaje' : 'Crea tu cuenta ahora'}
        </p>

        {error && (
          <div className="p-3 mb-4 rounded-lg bg-red-500/10 border border-red-500/20 text-red-500 text-sm text-center">
            {error}
          </div>
        )}

        <form onSubmit={handleSubmit}>
          {!isLogin && (
            <div className="input-group">
              <label>Nombre Completo</label>
              <input 
                type="text" 
                value={fullName} 
                onChange={(e) => setFullName(e.target.value)} 
                required 
                placeholder="Juan Pérez"
              />
            </div>
          )}
          <div className="input-group">
            <label>Email</label>
            <input 
              type="email" 
              value={email} 
              onChange={(e) => setEmail(e.target.value)} 
              required 
              placeholder="correo@ejemplo.com"
            />
          </div>
          <div className="input-group">
            <label>Contraseña</label>
            <input 
              type="password" 
              value={password} 
              onChange={(e) => setPassword(e.target.value)} 
              required 
              placeholder="••••••••"
            />
          </div>
          <button type="submit" className="btn btn-primary w-full justify-center mt-4">
            {isLogin ? <LogIn size={20} /> : <UserPlus size={20} />}
            {isLogin ? 'Iniciar Sesión' : 'Registrarse'}
          </button>
        </form>

        <div className="mt-8 text-center">
          <button 
            onClick={() => setIsLogin(!isLogin)}
            className="text-[var(--text-secondary)] hover:text-[var(--accent-primary)] text-sm transition-colors"
          >
            {isLogin ? '¿No tienes cuenta? Registrate' : '¿Ya tienes cuenta? Inicia sesión'}
          </button>
        </div>
      </motion.div>
    </div>
  );
};

export default LoginPage;
