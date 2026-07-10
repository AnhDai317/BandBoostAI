import { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';

const Register = () => {
    const navigate = useNavigate();
    const [isSignUp, setIsSignUp] = useState(false); // Default to Sign In for quicker flow
    const [formData, setFormData] = useState({ fullName: '', email: '', password: '' });
    const [showPassword, setShowPassword] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const [errorMsg, setErrorMsg] = useState('');
    const [errors, setErrors] = useState({});

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
        setErrorMsg('');
        setErrors({ ...errors, [name]: '' });
    };

    const validateForm = () => {
        const newErrors = {};
        if (isSignUp && !formData.fullName.trim()) {
            newErrors.fullName = "Vui lòng nhập họ và tên";
        }
        if (!formData.email.trim()) {
            newErrors.email = "Vui lòng nhập email";
        } else if (!/\S+@\S+\.\S+/.test(formData.email)) {
            newErrors.email = "Email không hợp lệ";
        }
        if (!formData.password) {
            newErrors.password = "Vui lòng nhập mật khẩu";
        } else if (formData.password.length < 6) {
            newErrors.password = "Mật khẩu phải có ít nhất 6 ký tự";
        }
        
        setErrors(newErrors);
        return Object.keys(newErrors).length === 0;
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        if (!validateForm()) return;

        setIsLoading(true);
        setErrorMsg('');

        const endpoint = isSignUp ? "register" : "login";
        const body = isSignUp 
            ? { fullName: formData.fullName, email: formData.email, password: formData.password }
            : { email: formData.email, password: formData.password };

        try {
            const response = await fetch(`http://localhost:5229/api/Auth/${endpoint}`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json"
                },
                body: JSON.stringify(body)
            });

            const data = await response.json();

            if (response.ok) {
                if (isSignUp) {
                    alert("Đăng ký tài khoản thành công! Hãy đăng nhập.");
                    setIsSignUp(false);
                    setFormData({ fullName: '', email: formData.email, password: '' });
                } else {
                    // Save credentials
                    localStorage.setItem("token", data.token);
                    localStorage.setItem("user", JSON.stringify({
                        fullName: data.fullName,
                        email: data.email,
                        role: data.role
                    }));
                    // Go to dashboard
                    navigate("/dashboard");
                }
            } else {
                setErrorMsg(data.message || (isSignUp ? "Đăng ký thất bại" : "Đăng nhập thất bại"));
            }
        } catch (error) {
            console.error("Lỗi kết nối:", error);
            setErrorMsg("Không thể kết nối đến máy chủ.");
        } finally {
            setIsLoading(false);
        }
    };

    return (
        <main className="flex-grow flex flex-col md:flex-row h-screen bg-slate-50">
            {/* Left branding panel */}
            <div className="hidden md:flex md:w-1/2 lg:w-5/12 bg-gradient-to-br from-indigo-950 via-violet-900 to-indigo-900 relative overflow-hidden items-center justify-center p-12">
                <div className="absolute inset-0 opacity-10 bg-[radial-gradient(#fff_1px,transparent_1px)] [background-size:16px_16px]"></div>
                <div className="relative z-10 text-center max-w-md">
                    <div className="mb-8 inline-flex items-center justify-center p-4 bg-white/10 rounded-full backdrop-blur-md border border-white/20 shadow-inner">
                        <span className="material-symbols-outlined text-white text-5xl animate-pulse">model_training</span>
                    </div>
                    <h1 className="text-white text-4xl font-extrabold mb-4 leading-tight tracking-tight">
                        Kiểm tra IELTS bằng trí tuệ nhân tạo.
                    </h1>
                    <p className="text-white/80 text-lg font-light">
                        Nhận đánh giá tức thì và chi tiết dựa theo 4 tiêu chí chuẩn IELTS của Cambridge.
                    </p>
                </div>
            </div>

            {/* Right form panel */}
            <div className="w-full md:w-1/2 lg:w-7/12 bg-white flex items-center justify-center p-6 md:p-12 relative overflow-y-auto">
                <div className="absolute top-6 right-6">
                    <Link to="/" className="text-sm font-semibold text-violet-600 hover:text-violet-800 transition-colors flex items-center gap-1">
                        <span className="material-symbols-outlined text-base">home</span> Về trang chủ
                    </Link>
                </div>

                <div className="w-full max-w-md space-y-8">
                    <div className="text-center md:text-left">
                        <h2 className="text-3xl font-black text-slate-900 tracking-tight">
                            {isSignUp ? "Tạo tài khoản mới" : "Chào mừng trở lại!"}
                        </h2>
                        <p className="text-slate-500 mt-2">
                            {isSignUp ? "Hãy bắt đầu hành trình chinh phục IELTS 7.0+ hôm nay." : "Đăng nhập để tiếp tục luyện tập và xem kết quả của bạn."}
                        </p>
                    </div>

                    {/* Toggle Tabs */}
                    <div className="flex border-b border-slate-200">
                        <button
                            type="button"
                            onClick={() => { setIsSignUp(false); setErrorMsg(''); }}
                            className={`flex-1 pb-3 text-center font-bold text-sm border-b-2 transition-all ${!isSignUp ? 'border-violet-600 text-violet-600' : 'border-transparent text-slate-400 hover:text-slate-600'}`}
                        >
                            Đăng Nhập
                        </button>
                        <button
                            type="button"
                            onClick={() => { setIsSignUp(true); setErrorMsg(''); }}
                            className={`flex-1 pb-3 text-center font-bold text-sm border-b-2 transition-all ${isSignUp ? 'border-violet-600 text-violet-600' : 'border-transparent text-slate-400 hover:text-slate-600'}`}
                        >
                            Đăng Ký
                        </button>
                    </div>

                    {errorMsg && (
                        <div className="p-4 bg-rose-50 border-l-4 border-rose-500 text-rose-700 rounded-r-xl flex items-center gap-2 animate-shake">
                            <span className="material-symbols-outlined">error</span>
                            <p className="text-sm font-medium">{errorMsg}</p>
                        </div>
                    )}

                    <form onSubmit={handleSubmit} className="space-y-6" noValidate>
                        {/* Full Name (Sign Up only) */}
                        {isSignUp && (
                            <div>
                                <label className="block text-sm font-semibold text-slate-700 mb-1" htmlFor="fullName">Họ và tên</label>
                                <input
                                    type="text" id="fullName" name="fullName"
                                    value={formData.fullName} onChange={handleInputChange}
                                    className={`block w-full bg-white px-4 py-3 border rounded-xl text-slate-900 caret-violet-600 placeholder:text-slate-400 focus:ring-4 outline-none transition-all ${errors.fullName ? 'border-rose-500 bg-rose-50/20 focus:ring-rose-500/10' : 'border-slate-200 focus:ring-violet-500/10 focus:border-violet-600'}`}
                                    placeholder="Nguyễn Văn A"
                                />
                                {errors.fullName && (
                                    <p className="mt-1 text-xs text-rose-600 flex items-center gap-1">
                                        <span className="material-symbols-outlined text-xs">error</span> {errors.fullName}
                                    </p>
                                )}
                            </div>
                        )}

                        {/* Email */}
                        <div>
                            <label className="block text-sm font-semibold text-slate-700 mb-1" htmlFor="email">Địa chỉ Email</label>
                            <input
                                type="email" id="email" name="email"
                                value={formData.email} onChange={handleInputChange}
                                className={`block w-full bg-white px-4 py-3 border rounded-xl text-slate-900 caret-violet-600 placeholder:text-slate-400 focus:ring-4 outline-none transition-all ${errors.email ? 'border-rose-500 bg-rose-50/20 focus:ring-rose-500/10' : 'border-slate-200 focus:ring-violet-500/10 focus:border-violet-600'}`}
                                placeholder="name@example.com"
                            />
                            {errors.email && (
                                <p className="mt-1 text-xs text-rose-600 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-xs">error</span> {errors.email}
                                </p>
                            )}
                        </div>

                        {/* Password */}
                        <div>
                            <label className="block text-sm font-semibold text-slate-700 mb-1" htmlFor="password">Mật khẩu</label>
                            <div className="relative">
                                <input
                                    type={showPassword ? "text" : "password"} id="password" name="password"
                                    value={formData.password} onChange={handleInputChange}
                                    className={`block w-full bg-white px-4 py-3 border rounded-xl text-slate-900 caret-violet-600 placeholder:text-slate-400 focus:ring-4 outline-none transition-all pr-12 ${errors.password ? 'border-rose-500 bg-rose-50/20 focus:ring-rose-500/10' : 'border-slate-200 focus:ring-violet-500/10 focus:border-violet-600'}`}
                                    placeholder="••••••••"
                                />
                                <button
                                    type="button"
                                    onClick={() => setShowPassword(!showPassword)}
                                    className="absolute inset-y-0 right-0 pr-4 flex items-center text-slate-400 hover:text-slate-600"
                                >
                                    <span className="material-symbols-outlined text-xl">
                                        {showPassword ? 'visibility' : 'visibility_off'}
                                    </span>
                                </button>
                            </div>
                            {errors.password && (
                                <p className="mt-1 text-xs text-rose-600 flex items-center gap-1">
                                    <span className="material-symbols-outlined text-xs">error</span> {errors.password}
                                </p>
                            )}
                        </div>

                        {/* Submit Button */}
                        <button
                            type="submit"
                            disabled={isLoading}
                            className={`w-full flex justify-center py-3.5 px-4 border border-transparent rounded-xl shadow-md text-white font-bold bg-gradient-to-r from-violet-600 to-indigo-600 hover:from-violet-700 hover:to-indigo-700 focus:outline-none focus:ring-4 focus:ring-violet-500/20 transition-all transform active:scale-98 ${isLoading ? 'opacity-70 cursor-not-allowed' : ''}`}
                        >
                            {isLoading ? (
                                <span className="flex items-center gap-2">
                                    <svg className="animate-spin h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" />
                                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                                    </svg>
                                    Đang xử lý...
                                </span>
                            ) : (
                                isSignUp ? 'Đăng Ký Ngay' : 'Đăng Nhập'
                            )}
                        </button>
                    </form>
                </div>
            </div>
        </main>
    );
};

export default Register;
