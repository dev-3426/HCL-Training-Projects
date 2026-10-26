const products = [
  {id:1,name:'Neem & Turmeric Face Wash',category:'Face Care',price:199,tag:'BESTSELLER',description:'A refreshing daily cleanse',image:'https://images.unsplash.com/photo-1556228720-195a672e8a03?auto=format&fit=crop&w=700&q=80'},
  {id:2,name:'Aloe Vera Gel',category:'Face Care',price:149,tag:'DAILY ESSENTIAL',description:'Lightweight, soothing hydration',image:'https://images.unsplash.com/photo-1608248543803-ba4f8c70ae0b?auto=format&fit=crop&w=700&q=80'},
  {id:3,name:'Turmeric Face Cream',category:'Face Care',price:249,tag:'NOURISHING',description:'A soft, comforting moisturiser',image:'https://images.unsplash.com/photo-1601049541289-9b1b7bbbfe19?auto=format&fit=crop&w=700&q=80'},
  {id:4,name:'Rose Water Toner',category:'Face Care',price:129,tag:'FRESH PICK',description:'A gentle botanical refresh',image:'https://images.unsplash.com/photo-1608571423902-eed4a5ad8108?auto=format&fit=crop&w=700&q=80'},
  {id:5,name:'Sandalwood Face Pack',category:'Face Care',price:299,tag:'WEEKLY RITUAL',description:'A clay-inspired skin ritual',image:'https://images.unsplash.com/photo-1596755389378-c31d21fd1273?auto=format&fit=crop&w=700&q=80'},
  {id:6,name:'Multani Mitti Face Scrub',category:'Face Care',price:179,tag:'GENTLE EXFOLIATION',description:'A fresh-feeling skin polish',image:'https://images.unsplash.com/photo-1571781926291-c477ebfd024b?auto=format&fit=crop&w=700&q=80'},
  {id:7,name:'Amla Hair Oil',category:'Hair Care',price:229,tag:'TRADITIONAL FAVOURITE',description:'A relaxing scalp massage ritual',image:'https://images.unsplash.com/photo-1608571423902-eed4a5ad8108?auto=format&fit=crop&w=700&q=80'},
  {id:8,name:'Amla & Bhringraj Shampoo',category:'Hair Care',price:279,tag:'HAIR CARE',description:'A botanical-inspired hair wash',image:'https://images.unsplash.com/photo-1608248543803-ba4f8c70ae0b?auto=format&fit=crop&w=700&q=80'},
  {id:9,name:'Hibiscus Hair Conditioner',category:'Hair Care',price:249,tag:'SOFT & SMOOTH',description:'For softer, easier-to-manage hair',image:'https://images.unsplash.com/photo-1601049541289-9b1b7bbbfe19?auto=format&fit=crop&w=700&q=80'},
  {id:10,name:'Neem & Tulsi Soap',category:'Body Care',price:99,tag:'EVERYDAY CARE',description:'A botanical bathing bar',image:'https://images.unsplash.com/photo-1600857544200-b2f666a9a2ec?auto=format&fit=crop&w=700&q=80'},
  {id:11,name:'Coconut Body Lotion',category:'Body Care',price:299,tag:'DAILY MOISTURE',description:'Comforting moisture for your skin',image:'https://images.unsplash.com/photo-1608248543803-ba4f8c70ae0b?auto=format&fit=crop&w=700&q=80'},
  {id:12,name:'Coffee Body Scrub',category:'Body Care',price:249,tag:'BODY RITUAL',description:'A coffee-inspired body polish',image:'https://images.unsplash.com/photo-1571781926291-c477ebfd024b?auto=format&fit=crop&w=700&q=80'},
  {id:13,name:'Honey & Shea Lip Balm',category:'Lip & Eye Care',price:99,tag:'POCKET FAVOURITE',description:'A soft touch of lip moisture',image:'https://images.unsplash.com/photo-1586495777744-4413f21062fa?auto=format&fit=crop&w=700&q=80'},
  {id:14,name:'Saffron Face Serum',category:'Face Care',price:399,tag:'GLOW RITUAL',description:'A lightweight daily serum',image:'https://images.unsplash.com/photo-1601049541289-9b1b7bbbfe19?auto=format&fit=crop&w=700&q=80'},
  {id:15,name:'Cucumber Under-Eye Gel',category:'Lip & Eye Care',price:199,tag:'COOLING CARE',description:'A refreshing under-eye ritual',image:'https://images.unsplash.com/photo-1556228720-195a672e8a03?auto=format&fit=crop&w=700&q=80'}
];

let activeCategory = 'All';
let cart = [];
const productGrid = document.getElementById('productGrid');
const searchInput = document.getElementById('productSearch');
const noResults = document.getElementById('noResults');
const cartToast = bootstrap.Toast.getOrCreateInstance(document.getElementById('cartToast'), {delay:2200});
const money = amount => new Intl.NumberFormat('en-IN',{style:'currency',currency:'INR',maximumFractionDigits:0}).format(amount);

function renderProducts(){
  const searchTerm = searchInput.value.trim().toLowerCase();
  const filtered = products.filter(product => {
    const matchesCategory = activeCategory === 'All' || product.category === activeCategory;
    const matchesSearch = `${product.name} ${product.category} ${product.description}`.toLowerCase().includes(searchTerm);
    return matchesCategory && matchesSearch;
  });
  productGrid.innerHTML = filtered.map(product => `
    <div class="col-6 col-lg-4 col-xl-3">
      <article class="product-card">
        <div class="product-image-wrap">
          <img class="product-image" src="${product.image}" alt="${product.name}" loading="lazy" onerror="this.style.display='none';this.nextElementSibling.classList.remove('d-none')">
          <div class="product-placeholder d-none"><i class="bi bi-flower2"></i></div>
          <span class="product-tag">${product.tag}</span>
          <button class="quick-add" type="button" data-add="${product.id}"><i class="bi bi-plus-lg me-1"></i> Add to bag</button>
        </div>
        <div class="product-info">
          <div class="product-category">${product.category}</div>
          <h3 class="product-name">${product.name}</h3>
          <div class="product-bottom"><span class="product-price">${money(product.price)}</span><button class="mobile-add" type="button" data-add="${product.id}" aria-label="Add ${product.name} to bag"><i class="bi bi-plus-lg"></i></button><span class="product-rating"><i class="bi bi-stars me-1"></i>Botanical pick</span></div>
        </div>
      </article>
    </div>`).join('');
  noResults.classList.toggle('d-none', filtered.length !== 0);
  productGrid.classList.toggle('d-none', filtered.length === 0);
}

function addToCart(id){
  const product = products.find(item => item.id === id);
  const existing = cart.find(item => item.id === id);
  if(existing) existing.quantity += 1; else cart.push({...product,quantity:1});
  renderCart();
  document.getElementById('toastText').textContent = `${product.name} added to your bag`;
  cartToast.show();
}
function changeQuantity(id,change){
  const item = cart.find(product => product.id === id);
  if(!item) return;
  item.quantity += change;
  if(item.quantity <= 0) cart = cart.filter(product => product.id !== id);
  renderCart();
}
function renderCart(){
  const cartItems = document.getElementById('cartItems');
  const count = cart.reduce((sum,item)=>sum+item.quantity,0);
  const subtotal = cart.reduce((sum,item)=>sum+item.price*item.quantity,0);
  document.getElementById('cartCount').textContent = count;
  document.getElementById('cartItemLabel').textContent = `(${count})`;
  document.getElementById('cartSubtotal').textContent = money(subtotal);
  document.getElementById('checkoutBtn').disabled = cart.length === 0;
  document.getElementById('checkoutMessage').textContent = '';
  if(!cart.length){
    cartItems.innerHTML = '<div class="empty-cart text-center py-5"><i class="bi bi-bag-heart"></i><h5>Your bag is taking a little break.</h5><p>Explore the collection and add a favourite.</p></div>';
    return;
  }
  cartItems.innerHTML = cart.map(item=>`<div class="cart-line"><img src="${item.image}" alt="${item.name}" onerror="this.style.opacity='.25'"><div><div class="cart-line-name">${item.name}</div><div class="cart-line-price">${money(item.price)}</div><div class="qty-control"><button type="button" data-qty="${item.id}" data-change="-1" aria-label="Decrease quantity">−</button><span>${item.quantity}</span><button type="button" data-qty="${item.id}" data-change="1" aria-label="Increase quantity">+</button></div><button type="button" class="remove-item" data-remove="${item.id}">Remove</button></div><div class="cart-line-total">${money(item.price*item.quantity)}</div></div>`).join('');
}

productGrid.addEventListener('click',event=>{const button=event.target.closest('[data-add]');if(button)addToCart(Number(button.dataset.add));});
document.querySelectorAll('.filter-btn').forEach(button=>button.addEventListener('click',()=>{
  activeCategory=button.dataset.category;
  document.querySelectorAll('.filter-btn').forEach(btn=>btn.classList.toggle('active',btn===button));
  renderProducts();
}));
searchInput.addEventListener('input',renderProducts);
document.getElementById('cartItems').addEventListener('click',event=>{
  const qtyButton=event.target.closest('[data-qty]');
  const removeButton=event.target.closest('[data-remove]');
  if(qtyButton)changeQuantity(Number(qtyButton.dataset.qty),Number(qtyButton.dataset.change));
  if(removeButton){cart=cart.filter(item=>item.id!==Number(removeButton.dataset.remove));renderCart();}
});
document.getElementById('checkoutBtn').addEventListener('click',()=>{
  document.getElementById('checkoutMessage').textContent='This is a front-end demo. Online payment is not connected yet.';
});
document.getElementById('newsletterForm').addEventListener('submit',event=>{
  event.preventDefault();
  document.getElementById('newsletterMessage').textContent='Thanks for joining us! This demo does not store email addresses.';
  event.target.reset();
});
document.querySelectorAll('[data-footer-category]').forEach(link=>link.addEventListener('click',()=>{
  activeCategory=link.dataset.footerCategory;
  document.querySelectorAll('.filter-btn').forEach(btn=>btn.classList.toggle('active',btn.dataset.category===activeCategory));
  renderProducts();
}));
document.getElementById('currentYear').textContent=new Date().getFullYear();
renderProducts();
renderCart();
