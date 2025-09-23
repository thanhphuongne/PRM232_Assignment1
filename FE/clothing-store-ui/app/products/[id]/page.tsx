'use client';

import { useEffect, useState } from 'react';
import { useParams, useRouter } from 'next/navigation';
import Link from 'next/link';
import { getProduct, deleteProduct, Product } from '../../../lib/api';

export default function ProductDetail() {
  const params = useParams();
  const router = useRouter();
  const [product, setProduct] = useState<Product | null>(null);
  const [loading, setLoading] = useState(true);

  const id = parseInt(params.id as string);

  useEffect(() => {
    const fetchProduct = async () => {
      try {
        const data = await getProduct(id);
        setProduct(data);
      } catch (error) {
        console.error('Failed to fetch product:', error);
      } finally {
        setLoading(false);
      }
    };
    fetchProduct();
  }, [id]);

  const handleDelete = async () => {
    if (confirm('Are you sure you want to delete this product?')) {
      try {
        await deleteProduct(id);
        router.push('/');
      } catch (error) {
        console.error('Failed to delete product:', error);
      }
    }
  };

  if (loading) return <div className="text-center mt-10">Loading...</div>;
  if (!product) return <div className="text-center mt-10">Product not found</div>;

  return (
    <div className="container mx-auto p-4">
      <div className="mb-4">
        <Link href="/" className="text-blue-500 hover:underline">← Back to Products</Link>
      </div>
      <div className="max-w-2xl mx-auto">
        {product.imageUrl && (
          <img src={product.imageUrl} alt={product.name} className="w-full h-64 object-cover mb-4 rounded" />
        )}
        <h1 className="text-3xl font-bold mb-4">{product.name}</h1>
        <p className="text-gray-600 mb-4">{product.description}</p>
        <p className="text-2xl font-bold mb-6">${product.price}</p>
        <div className="flex gap-4">
          <Link href={`/products/${product.id}/edit`} className="bg-yellow-500 text-white px-4 py-2 rounded hover:bg-yellow-600">
            Edit
          </Link>
          <button onClick={handleDelete} className="bg-red-500 text-white px-4 py-2 rounded hover:bg-red-600">
            Delete
          </button>
        </div>
      </div>
    </div>
  );
}