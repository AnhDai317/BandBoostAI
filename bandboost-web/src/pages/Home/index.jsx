import React from 'react';
import { Link } from 'react-router-dom';

const Home = () => {
    return (
        <div className="min-h-screen bg-surface">
            {/* TopAppBar */}
            <header className="bg-surface/95 border-b border-outline-variant/30 docked full-width top-0 sticky z-50 backdrop-blur-sm">
                <div className="flex justify-between items-center w-full px-margin-desktop max-w-[1440px] mx-auto h-20">
                    <div className="flex items-center gap-base">
                        <span className="font-display-md text-headline-md font-bold text-secondary">BandBoost AI</span>
                    </div>
                    <nav className="hidden md:flex items-center gap-lg">
                        <a className="text-secondary font-bold border-b-2 border-secondary pb-1 font-label-caps text-label-caps" href="#">Dashboard</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors duration-300 font-label-caps text-label-caps" href="#">Practice Tests</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors duration-300 font-label-caps text-label-caps" href="#">AI Feedback</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors duration-300 font-label-caps text-label-caps" href="#">Vocabulary</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors duration-300 font-label-caps text-label-caps" href="#">Simulations</a>
                    </nav>
                    <div className="flex items-center gap-md">
                        {/* Chuyển hướng người dùng sang trang Register khi bấm nút */}
                        <Link to="/register" className="hidden lg:flex items-center px-lg py-sm bg-primary-container text-on-primary-container rounded-full font-bold active:scale-95 transition-transform duration-200">
                            Start Exam
                        </Link>
                        <div className="w-10 h-10 rounded-full border border-outline-variant p-0.5">
                            <img alt="Academic Profile" className="w-full h-full rounded-full object-cover" src="https://lh3.googleusercontent.com/aida-public/AB6AXuCpcOct4Bvkc7k8Jh3ffLbMN5gkkCvytjwfuYosneDUFIouKtdCUsqGl65wECTaSEWCqgeuXymsOPZYMaZuHtUiElfP3Ey_6iXIt_Iq53c2WL1bWgSG7G2sJRrhhvhxj-v437XV1G6LL9w3B9ojeI4YS4gj13n8Yz6p0dNA85ACvlf0uXHIzm_jBNif8zsfWedCWEHywN6KeAXK3NG8l6jFK6F3UlmdsDA7yuQ4MoMy8CwtcgklNVmZznQ3qInlEHItODpnWqhnJexo" />
                        </div>
                    </div>
                </div>
            </header>

            <main className="relative">
                {/* Hero Section */}
                <section className="relative pt-24 pb-32 px-margin-desktop max-w-[1440px] mx-auto overflow-hidden">
                    <div className="grid lg:grid-cols-2 gap-xl items-center">
                        <div className="z-10">
                            <div className="inline-flex items-center gap-sm px-md py-xs rounded-full bg-surface-container border border-outline-variant/50 mb-lg">
                                <span className="material-symbols-outlined text-secondary text-base">school</span>
                                <span className="font-label-caps text-label-caps text-on-surface-variant">Hệ thống luyện thi IELTS thông minh</span>
                            </div>
                            <h1 className="font-display-lg text-display-lg text-on-surface mb-md">
                                Nâng tầm <span className="text-secondary">IELTS</span> <br />
                                với công nghệ giáo dục
                            </h1>
                            <p className="font-body-lg text-body-lg text-on-surface-variant mb-xl max-w-xl">
                                BandBoost AI cung cấp lộ trình luyện thi chuẩn học thuật, tích hợp công nghệ phân tích chuyên sâu giúp bạn đánh giá năng lực chính xác và tối ưu điểm số hiệu quả.
                            </p>
                            <div className="flex flex-wrap gap-md">
                                <Link to="/register" className="px-xl py-md bg-secondary text-on-secondary-fixed rounded-xl font-bold text-lg hover:bg-secondary/90 shadow-md transition-all">
                                    Bắt đầu học ngay
                                </Link>
                                <button className="px-xl py-md bg-surface-container border border-outline-variant text-on-surface rounded-xl font-bold text-lg hover:bg-surface-container-high transition-all flex items-center gap-sm shadow-sm">
                                    <span className="material-symbols-outlined">play_circle</span>
                                    Xem cách hoạt động
                                </button>
                            </div>
                        </div>
                        <div className="relative">
                            {/* Academic/Educational Image */}
                            <div className="relative z-10 bg-surface-container rounded-3xl p-2 aspect-[4/3] flex items-center justify-center border border-outline-variant shadow-lg">
                                <img alt="Student studying in library" className="w-full h-full object-cover rounded-2xl" src="https://images.unsplash.com/photo-1523240795612-9a054b0db644?ixlib=rb-4.0.3&auto=format&fit=crop&w=1000&q=80" />
                                {/* Solid Data Cards */}
                                <div className="absolute -top-6 -right-6 bg-surface-container-high border border-outline-variant p-md rounded-xl shadow-lg z-20">
                                    <div className="flex items-center gap-sm">
                                        <span className="material-symbols-outlined text-secondary" style={{ fontVariationSettings: "'FILL' 1" }}>analytics</span>
                                        <div>
                                            <p className="font-label-caps text-[10px] text-on-surface-variant uppercase">Độ chính xác</p>
                                            <p className="font-bold text-on-surface">Tiêu chuẩn Cambridge</p>
                                        </div>
                                    </div>
                                </div>
                                <div className="absolute -bottom-8 -left-8 bg-surface-container-high border border-outline-variant p-md rounded-xl shadow-lg z-20">
                                    <div className="flex items-center gap-sm">
                                        <span className="material-symbols-outlined text-primary" style={{ fontVariationSettings: "'FILL' 1" }}>assignment_turned_in</span>
                                        <div>
                                            <p className="font-label-caps text-[10px] text-on-surface-variant uppercase">Đánh giá toàn diện</p>
                                            <p className="font-bold text-on-surface">4 kỹ năng</p>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>

                {/* Features Section */}
                <section className="py-32 px-margin-desktop max-w-[1440px] mx-auto">
                    <div className="text-center mb-xl">
                        <h2 className="font-headline-lg text-headline-lg text-on-surface mb-sm uppercase tracking-widest">Tính năng học tập</h2>
                        <p className="text-on-surface-variant font-body-md text-body-md">Phương pháp khoa học, kết quả thực tế</p>
                    </div>
                    <div className="grid md:grid-cols-3 gap-gutter">
                        {/* Card 1 */}
                        <div className="bg-surface-container border border-outline-variant p-xl rounded-3xl shadow-sm hover:shadow-md transition-all duration-300">
                            <div className="w-16 h-16 rounded-2xl bg-secondary/10 flex items-center justify-center mb-lg border border-secondary/20">
                                <span className="material-symbols-outlined text-secondary text-4xl" style={{ fontVariationSettings: "'FILL' 1" }}>record_voice_over</span>
                            </div>
                            <h3 className="font-headline-md text-headline-md text-on-surface mb-md">Phân tích phát âm</h3>
                            <p className="text-on-surface-variant font-body-md text-body-md mb-lg">
                                Hệ thống nhận diện và đối chiếu phát âm, ngữ điệu theo tiêu chuẩn bảng mẫu âm quốc tế IPA và tiêu chí Cambridge.
                            </p>
                            <a className="text-secondary font-bold inline-flex items-center gap-xs hover:underline" href="#">
                                Khám phá thêm <span className="material-symbols-outlined">arrow_forward</span>
                            </a>
                        </div>
                        {/* Card 2 */}
                        <div className="bg-surface-container border border-outline-variant p-xl rounded-3xl shadow-sm hover:shadow-md transition-all duration-300">
                            <div className="w-16 h-16 rounded-2xl bg-primary/10 flex items-center justify-center mb-lg border border-primary/20">
                                <span className="material-symbols-outlined text-primary text-4xl" style={{ fontVariationSettings: "'FILL' 1" }}>edit_note</span>
                            </div>
                            <h3 className="font-headline-md text-headline-md text-on-surface mb-md">Chấm điểm Writing</h3>
                            <p className="text-on-surface-variant font-body-md text-body-md mb-lg">
                                Đánh giá bài viết chi tiết dựa trên 4 tiêu chí chấm điểm IELTS chính thức, kèm gợi ý cải thiện cấu trúc và từ vựng.
                            </p>
                            <a className="text-primary font-bold inline-flex items-center gap-xs hover:underline" href="#">
                                Khám phá thêm <span className="material-symbols-outlined">arrow_forward</span>
                            </a>
                        </div>
                        {/* Card 3 */}
                        <div className="bg-surface-container border border-outline-variant p-xl rounded-3xl shadow-sm hover:shadow-md transition-all duration-300">
                            <div className="w-16 h-16 rounded-2xl bg-tertiary/10 flex items-center justify-center mb-lg border border-tertiary/20">
                                <span className="material-symbols-outlined text-tertiary text-4xl" style={{ fontVariationSettings: "'FILL' 1" }}>menu_book</span>
                            </div>
                            <h3 className="font-headline-md text-headline-md text-on-surface mb-md">Lộ trình cá nhân hóa</h3>
                            <p className="text-on-surface-variant font-body-md text-body-md mb-lg">
                                Thiết kế chương trình học chuyên biệt dựa trên năng lực hiện tại, giúp củng cố kiến thức và tối ưu thời gian ôn luyện.
                            </p>
                            <a className="text-tertiary font-bold inline-flex items-center gap-xs hover:underline" href="#">
                                Khám phá thêm <span className="material-symbols-outlined">arrow_forward</span>
                            </a>
                        </div>
                    </div>
                </section>

                {/* Call to Action */}
                <section className="py-32 px-margin-desktop max-w-[1440px] mx-auto">
                    <div className="bg-surface-container border border-outline-variant rounded-[3rem] p-xl relative overflow-hidden flex flex-col items-center text-center shadow-lg">
                        <h2 className="font-display-md text-display-md text-on-surface mb-lg z-10 max-w-2xl">
                            Sẵn sàng cho hành trình chinh phục IELTS?
                        </h2>
                        <p className="font-body-lg text-body-lg text-on-surface-variant mb-xl max-w-xl z-10">
                            Bắt đầu với bài đánh giá năng lực toàn diện và nhận lộ trình học tập được thiết kế riêng cho bạn.
                        </p>
                        <div className="z-10 flex gap-md">
                            <Link to="/register" className="px-xl py-md bg-primary text-on-primary-fixed rounded-full font-bold text-lg hover:bg-primary/90 shadow-md transition-all">
                                Làm bài test đầu vào
                            </Link>
                        </div>
                    </div>
                </section>
            </main>

            {/* Footer */}
            <footer className="bg-surface-container-lowest border-t border-outline-variant/30">
                <div className="w-full py-xl px-margin-desktop flex flex-col md:flex-row justify-between items-center gap-md max-w-[1440px] mx-auto">
                    <div className="flex flex-col items-center md:items-start gap-sm">
                        <span className="font-display-md text-headline-sm font-black text-primary">BandBoost AI</span>
                        <p className="font-body-sm text-body-sm text-on-surface-variant">© 2026 BandBoost AI. Engineering Academic Excellence.</p>
                    </div>
                    <nav className="flex flex-wrap justify-center gap-lg">
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors font-label-caps text-label-caps" href="#">Privacy Protocol</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors font-label-caps text-label-caps" href="#">Terms of Service</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors font-label-caps text-label-caps" href="#">Academic Integrity</a>
                        <a className="text-on-surface-variant hover:text-on-surface transition-colors font-label-caps text-label-caps" href="#">Institutional Support</a>
                    </nav>
                    <div className="flex gap-md">
                        <a className="w-10 h-10 rounded-full bg-surface-container border border-outline-variant flex items-center justify-center hover:bg-surface-container-high transition-all" href="#">
                            <span className="material-symbols-outlined text-lg text-on-surface-variant">language</span>
                        </a>
                        <a className="w-10 h-10 rounded-full bg-surface-container border border-outline-variant flex items-center justify-center hover:bg-surface-container-high transition-all" href="#">
                            <span className="material-symbols-outlined text-lg text-on-surface-variant">hub</span>
                        </a>
                        <a className="w-10 h-10 rounded-full bg-surface-container border border-outline-variant flex items-center justify-center hover:bg-surface-container-high transition-all" href="#">
                            <span className="material-symbols-outlined text-lg text-on-surface-variant">school</span>
                        </a>
                    </div>
                </div>
            </footer>
        </div>
    );
};

export default Home;