(() => {
  const root = document.documentElement;
  if (window.lucide) window.lucide.createIcons();
  const savedTheme = localStorage.getItem('nexus-theme');
  if (savedTheme === 'light') root.dataset.theme = 'light';
  document.querySelectorAll('.theme-toggle').forEach(button => button.addEventListener('click', () => {
    root.dataset.theme = root.dataset.theme === 'light' ? 'dark' : 'light';
    localStorage.setItem('nexus-theme', root.dataset.theme);
  }));

  document.querySelectorAll('[data-count]').forEach(el => {
    const end = Number(el.dataset.count || 0);
    if (matchMedia('(prefers-reduced-motion: reduce)').matches) return;
    const start = performance.now();
    const tick = now => {
      const p = Math.min(1, (now - start) / 650);
      el.textContent = String(Math.round(end * (1 - Math.pow(1 - p, 3))));
      if (p < 1) requestAnimationFrame(tick);
    };
    requestAnimationFrame(tick);
  });

  const chart = document.getElementById('departmentChart');
  if (chart && window.Chart) {
    const labels = JSON.parse(chart.dataset.labels || '[]');
    const values = JSON.parse(chart.dataset.values || '[]');
    new Chart(chart, { type: 'bar', data: { labels, datasets: [{ data: values, backgroundColor: ['#8a73ff','#53d8ed','#66dfbf','#eacb82','#ed8db4'], borderRadius: 5, borderSkipped: false, maxBarThickness: 27 }] }, options: { maintainAspectRatio: false, plugins: { legend: { display: false }, tooltip: { backgroundColor: '#151c30', titleColor: '#f5f7ff', bodyColor: '#aeb9cf', padding: 10, displayColors: false } }, scales: { x: { grid: { display: false }, ticks: { color: '#8995af', font: { size: 9 } }, border: { display: false } }, y: { beginAtZero: true, ticks: { color: '#74819a', stepSize: 1, precision: 0, font: { size: 9 } }, grid: { color: 'rgba(173,190,232,.08)' }, border: { display: false } } } } });
  }

  const canvas = document.getElementById('universe');
  if (!canvas || !window.THREE || matchMedia('(prefers-reduced-motion: reduce)').matches) return;
  try {
    const scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(42, 1, .1, 100);
    camera.position.z = 7.4;
    const renderer = new THREE.WebGLRenderer({ canvas, alpha: true, antialias: true, powerPreference: 'low-power' });
    renderer.setPixelRatio(Math.min(devicePixelRatio, 1.6));
    const group = new THREE.Group(); scene.add(group);
    const globe = new THREE.Mesh(new THREE.IcosahedronGeometry(1.47, 2), new THREE.MeshBasicMaterial({ color: 0x8474ff, wireframe: true, transparent: true, opacity: .14 }));
    group.add(globe);
    const points = [];
    const count = 44;
    for (let i = 0; i < count; i++) {
      const y = 1 - i / (count - 1) * 2;
      const radius = Math.sqrt(1 - y * y);
      const theta = Math.PI * (3 - Math.sqrt(5)) * i;
      points.push(new THREE.Vector3(Math.cos(theta) * radius, y, Math.sin(theta) * radius).multiplyScalar(1.5));
    }
    const nodeGeometry = new THREE.BufferGeometry().setFromPoints(points);
    const nodes = new THREE.Points(nodeGeometry, new THREE.PointsMaterial({ color: 0x7cdeee, size: .055, transparent: true, opacity: .85, sizeAttenuation: true }));
    group.add(nodes);
    const positions = [];
    for (let i = 0; i < points.length; i++) {
      const nearest = points.map((p, j) => ({ j, d: points[i].distanceToSquared(p) })).filter(v => v.j > i).sort((a, b) => a.d - b.d).slice(0, 2);
      for (const n of nearest) positions.push(...points[i].toArray(), ...points[n.j].toArray());
    }
    const linesGeometry = new THREE.BufferGeometry();
    linesGeometry.setAttribute('position', new THREE.Float32BufferAttribute(positions, 3));
    group.add(new THREE.LineSegments(linesGeometry, new THREE.LineBasicMaterial({ color: 0x7264e8, transparent: true, opacity: .2 })));
    const particlePositions = new Float32Array(180 * 3);
    for (let i = 0; i < particlePositions.length; i += 3) { particlePositions[i] = (Math.random() - .5) * 7; particlePositions[i + 1] = (Math.random() - .5) * 6; particlePositions[i + 2] = (Math.random() - .5) * 2; }
    const particleGeometry = new THREE.BufferGeometry(); particleGeometry.setAttribute('position', new THREE.BufferAttribute(particlePositions, 3));
    scene.add(new THREE.Points(particleGeometry, new THREE.PointsMaterial({ color: 0x8ba9ff, size: .018, transparent: true, opacity: .5 })));
    let pointerX = 0, pointerY = 0, frame = 0;
    const resize = () => { const rect = canvas.getBoundingClientRect(); if (!rect.width || !rect.height) return; renderer.setSize(rect.width, rect.height, false); camera.aspect = rect.width / rect.height; camera.updateProjectionMatrix(); };
    const move = e => { const r = canvas.getBoundingClientRect(); pointerX = ((e.clientX - r.left) / r.width - .5) * .42; pointerY = ((e.clientY - r.top) / r.height - .5) * .3; };
    window.addEventListener('resize', resize, { passive: true }); canvas.parentElement.addEventListener('pointermove', move, { passive: true }); resize();
    const animate = () => { frame = requestAnimationFrame(animate); group.rotation.y += .0014; group.rotation.x += (pointerY - group.rotation.x) * .014; group.rotation.y += pointerX * .001; renderer.render(scene, camera); };
    animate();
    document.addEventListener('visibilitychange', () => { if (document.hidden) cancelAnimationFrame(frame); else animate(); });
  } catch (_) { canvas.style.display = 'none'; }
})();
