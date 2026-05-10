import React, { useState } from 'react';

const Register = () => {
    // Quản lý state cho form
    const [formData, setFormData] = useState({ fullName: '', email: '', password: '' });
    const [showPassword, setShowPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [errorMsg, setErrorMsg] = useState('');
    const [errors, setErrors] = useState({}); // Thêm state lưu lỗi từng trường

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
        setErrorMsg(''); // Xóa lỗi chung
        setErrors({ ...errors, [name]: '' }); // Xóa lỗi của trường đang nhập
    };

    const validateForm = () => {
        const newErrors = {};
        if (!formData.fullName.trim()) newErrors.fullName = "Vui lòng nhập họ và tên";
        if (!formData.email.trim()) newErrors.email = "Vui lòng nhập email";
        else if (!/\S+@\S+\.\S+/.test(formData.email)) newErrors.email = "Email không hợp lệ";
        if (!formData.password) newErrors.password = "Vui lòng nhập mật khẩu";
        else if (formData.password.length < 6) newErrors.password = "Mật khẩu phải có ít nhất 6 ký tự";
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        
        if (!validateForm()) return; // Dừng lại nếu form không hợp lệ

        setIsLoading(true);

        // Chỗ này sau này sẽ gọi hàm từ folder src/services/authService.js
        console.log("Dữ liệu gửi đi:", formData);

        setTimeout(() => {
            setIsLoading(false);
            // Giả lập lỗi để test UI
            // setErrorMsg("Email này đã được đăng ký!"); 
        }, 1500);
    };

    return (
        <main className="flex-grow flex flex-col md:flex-row h-screen">
            {/* Cột trái: Branding */}
            <div className="hidden md:flex md:w-1/2 lg:w-5/12 bg-gradient-to-br from-[#1e1b4b] via-primary to-[#8b5cf6] relative overflow-hidden items-center justify-center p-10">
                <div className="relative z-10 text-center max-w-md">
                    <div className="mb-8 inline-flex items-center justify-center p-4 bg-white/10 rounded-full backdrop-blur-md">
                        <span className="material-symbols-outlined text-white text-5xl">model_training</span>
                    </div>
                    <h1 className="text-white text-4xl font-bold mb-4 leading-tight">
                        Boost your Band Score with AI precision.
                    </h1>
                    <p className="text-white/80 text-lg">
                        Join thousands of students achieving their IELTS goals through data-driven preparation.
                    </p>
                </div>
            </div>

            {/* Cột phải: Form */}
            <div className="w-full md:w-1/2 lg:w-7/12 bg-white flex items-center justify-center p-5 md:p-10 relative">
                <div className="w-full max-w-md space-y-8 mt-12 md:mt-0">
                    <div>
                        <h2 className="text-3xl font-bold text-on-surface mb-2">Create your account</h2>
                        <p className="text-gray-500">Start your journey to a 7.0+ band score today.</p>
                    </div>

                    <form onSubmit={handleSubmit} className="space-y-6" noValidate>
                        {/* Họ tên */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="fullName">Full Name</label>
                            <input
                                type="text" id="fullName" name="fullName"
                                value={formData.fullName} onChange={handleInputChange}
                                className={`block w-full px-4 py-2 border rounded-lg focus:ring-2 outline-none transition-all ${errors.fullName ? 'border-error bg-error-container/10 focus:ring-error/20' : 'border-gray-300 focus:ring-primary/20 focus:border-primary'}`}
                                placeholder="John Doe"
                            />
                            {errors.fullName && (
                                <p className="mt-1 text-sm text-error flex items-center gap-1">
                                    <span className="material-symbols-outlined text-[16px]">error</span> {errors.fullName}
                                </p>
                            )}
                        </div>

                        {/* Email */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="email">Email Address</label>
                            <input
                                type="email" id="email" name="email"
                                value={formData.email} onChange={handleInputChange}
                                className={`block w-full px-4 py-2 border rounded-lg focus:ring-2 outline-none transition-all ${errors.email || errorMsg ? 'border-error bg-error-container/10 focus:ring-error/20' : 'border-gray-300 focus:ring-primary/20 focus:border-primary'}`}
                                placeholder="you@example.com"
                            />
                            {(errors.email || errorMsg) && (
                                <p className="mt-1 text-sm text-error flex items-center gap-1">
                                    <span className="material-symbols-outlined text-[16px]">error</span> {errors.email || errorMsg}
                                </p>
                            )}
                        </div>

                        {/* Mật khẩu */}
                        <div>
                            <label className="block text-sm font-medium text-gray-700 mb-1" htmlFor="password">Password</label>
                            <div className="relative">
                                <input
                                    type={showPassword ? "text" : "password"} id="password" name="password"
                                    value={formData.password} onChange={handleInputChange}
                                    className={`block w-full px-4 py-2 border rounded-lg focus:ring-2 outline-none transition-all pr-10 ${errors.password ? 'border-error bg-error-container/10 focus:ring-error/20' : 'border-gray-300 focus:ring-primary/20 focus:border-primary'}`}
                                    placeholder="••••••••"
                                />
                                <button
                                    type="button"
                                    onClick={() => setShowPassword(!showPassword)}
                                    className="absolute inset-y-0 right-0 pr-3 flex items-center text-gray-400 hover:text-gray-600"
                                >
                                    <span className="material-symbols-outlined">
                                        {showPassword ? 'visibility' : 'visibility_off'}
                                    </span>
                                </button>
                            </div>
                            {errors.password && (
                                <p className="mt-1 text-sm text-error flex items-center gap-1">
                                    <span className="material-symbols-outlined text-[16px]">error</span> {errors.password}
                                </p>
                            )}
                        </div>

                        {/* Nút Submit */}
                        <button
                            type="submit"
                            disabled={isLoading}
                            className={`w-full flex justify-center py-3 px-4 border border-transparent rounded-xl shadow-sm text-white bg-gradient-to-r from-primary to-[#6d28d9] hover:from-[#2e1ea8] hover:to-[#5b21b6] focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-primary transition-all ${isLoading ? 'opacity-70 cursor-not-allowed' : ''}`}
                        >
                            {isLoading ? 'Processing...' : 'Sign Up'}
                        </button>
                    </form>
                </div>
            </div>
        </main>
    );
};

export default Register;