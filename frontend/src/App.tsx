import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './context/AuthContext';
import LoginPage from './pages/LoginPage';
import CoursesPage from './pages/CoursesPage';
import LessonsPage from './pages/LessonsPage';

const ProtectedRoute = ({ children }: { children: React.ReactNode }) => {
  const { isAuthenticated } = useAuth();
  return isAuthenticated ? <>{children}</> : <Navigate to="/login" />;
};

function App() {
  return (
    <Router>
      <AuthProvider>
        <Routes>
          <Route path="/login" element={<LoginPage />} />
          <Route path="/courses" element={<ProtectedRoute><CoursesPage /></ProtectedRoute>} />
          <Route path="/courses/:courseId/lessons" element={<ProtectedRoute><LessonsPage /></ProtectedRoute>} />
          <Route path="*" element={<Navigate to="/courses" />} />
        </Routes>
      </AuthProvider>
    </Router>
  );
}

export default App;
