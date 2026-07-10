import { Link, NavLink, useNavigate } from "react-router-dom";

const navigation = [
    { to: "/dashboard", label: "Lộ trình", icon: "route" },
    { to: "/vocabulary", label: "Từ vựng", icon: "style" },
    { to: "/practice", label: "Luyện kỹ năng", icon: "edit_note" }
];

const LearningShell = ({ children }) => {
    const navigate = useNavigate();
    const user = JSON.parse(localStorage.getItem("user") || "{}");

    const logout = () => {
        localStorage.removeItem("token");
        localStorage.removeItem("user");
        navigate("/");
    };

    return (
        <div className="min-h-screen bg-[#080d1b] text-slate-100">
            <header className="sticky top-0 z-50 border-b border-white/5 bg-[#080d1b]/90 backdrop-blur-xl">
                <div className="mx-auto flex h-18 max-w-7xl items-center justify-between px-4 py-4 md:px-8">
                    <div className="flex items-center gap-8">
                        <Link to="/dashboard" className="flex items-center gap-3">
                            <span className="grid h-10 w-10 place-items-center rounded-2xl bg-gradient-to-br from-violet-500 to-indigo-600 shadow-lg shadow-violet-950/60">
                                <span className="material-symbols-outlined text-xl">graphic_eq</span>
                            </span>
                            <span className="hidden text-lg font-extrabold tracking-tight sm:block">BandBoost <span className="text-violet-400">AI</span></span>
                        </Link>
                        <nav className="hidden items-center gap-1 md:flex">
                            {navigation.map(item => (
                                <NavLink
                                    key={item.to}
                                    to={item.to}
                                    className={({ isActive }) => `flex items-center gap-2 rounded-xl px-4 py-2 text-sm font-semibold transition ${isActive ? "bg-violet-500/15 text-violet-300" : "text-slate-400 hover:bg-white/5 hover:text-white"}`}
                                >
                                    <span className="material-symbols-outlined text-lg">{item.icon}</span>
                                    {item.label}
                                </NavLink>
                            ))}
                        </nav>
                    </div>
                    <div className="flex items-center gap-3">
                        <Link to="/onboarding" className="hidden rounded-xl p-2 text-slate-400 transition hover:bg-white/5 hover:text-white sm:block" title="Chỉnh mục tiêu">
                            <span className="material-symbols-outlined">tune</span>
                        </Link>
                        <div className="hidden text-right sm:block">
                            <p className="text-sm font-bold text-slate-200">{user.fullName || "Học viên"}</p>
                            <p className="text-[11px] text-slate-500">{user.email}</p>
                        </div>
                        <button onClick={logout} className="grid h-10 w-10 place-items-center rounded-xl border border-white/10 text-slate-400 transition hover:border-rose-400/30 hover:bg-rose-500/10 hover:text-rose-300" title="Đăng xuất">
                            <span className="material-symbols-outlined text-xl">logout</span>
                        </button>
                    </div>
                </div>
            </header>

            <main className="mx-auto max-w-7xl px-4 py-8 md:px-8 md:py-10">{children}</main>

            <nav className="fixed inset-x-4 bottom-4 z-50 flex items-center justify-around rounded-2xl border border-white/10 bg-slate-900/95 p-2 shadow-2xl backdrop-blur-xl md:hidden">
                {navigation.map(item => (
                    <NavLink key={item.to} to={item.to} className={({ isActive }) => `flex min-w-20 flex-col items-center gap-1 rounded-xl px-3 py-2 text-[11px] font-semibold ${isActive ? "bg-violet-500/15 text-violet-300" : "text-slate-500"}`}>
                        <span className="material-symbols-outlined text-xl">{item.icon}</span>
                        {item.label}
                    </NavLink>
                ))}
            </nav>
        </div>
    );
};

export default LearningShell;
