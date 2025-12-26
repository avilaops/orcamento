import os
import stripe
from django.conf import settings
from django.contrib.auth.decorators import login_required
from django.http import HttpResponse, JsonResponse
from django.shortcuts import render, redirect
from django.urls import reverse
from django.views.decorators.csrf import csrf_exempt
from django.views.decorators.http import require_POST
from .models import UserProfile

stripe.api_key = os.getenv('STRIPE_SECRET_KEY', '')

# Preços do Stripe (você vai criar esses no dashboard do Stripe)
STRIPE_PRICES = {
    'pro': os.getenv('STRIPE_PRICE_PRO', 'price_pro_monthly'),
    'enterprise': os.getenv('STRIPE_PRICE_ENTERPRISE', 'price_enterprise_monthly'),
}

@login_required
def checkout_session(request, plan):
    """Cria sessão de checkout do Stripe"""

    if plan not in ['pro', 'enterprise']:
        return redirect('pricing')

    try:
        profile = request.user.profile
    except UserProfile.DoesNotExist:
        profile = UserProfile.objects.create(user=request.user)

    # Criar sessão de checkout
    try:
        checkout_session = stripe.checkout.Session.create(
            customer_email=request.user.email,
            payment_method_types=['card'],
            line_items=[{
                'price': STRIPE_PRICES[plan],
                'quantity': 1,
            }],
            mode='subscription',
            success_url=request.build_absolute_uri(reverse('checkout_success')) + '?session_id={CHECKOUT_SESSION_ID}',
            cancel_url=request.build_absolute_uri(reverse('pricing')),
            metadata={
                'user_id': request.user.id,
                'plan': plan,
            },
        )
        return redirect(checkout_session.url)

    except Exception as e:
        return render(request, 'budget/checkout_error.html', {'error': str(e)})


@login_required
def checkout_success(request):
    """Página de sucesso após checkout"""
    session_id = request.GET.get('session_id')

    if session_id:
        try:
            session = stripe.checkout.Session.retrieve(session_id)

            # Atualizar perfil do usuário
            profile = request.user.profile
            plan = session.metadata.get('plan', 'pro')
            profile.plano = plan
            profile.stripe_customer_id = session.customer
            profile.stripe_subscription_id = session.subscription

            # Atualizar limites
            if plan == 'pro':
                profile.limite_transacoes = -1  # Ilimitado
                profile.limite_orcamentos = -1
            elif plan == 'enterprise':
                profile.limite_transacoes = -1
                profile.limite_orcamentos = -1

            profile.save()

        except Exception as e:
            print(f"Erro ao processar sessão: {e}")

    return render(request, 'budget/checkout_success.html')


@login_required
def checkout_cancel(request):
    """Página de cancelamento"""
    return render(request, 'budget/checkout_cancel.html')


@csrf_exempt
@require_POST
def stripe_webhook(request):
    """Webhook para receber eventos do Stripe"""

    payload = request.body
    sig_header = request.META.get('HTTP_STRIPE_SIGNATURE')
    endpoint_secret = os.getenv('STRIPE_WEBHOOK_SECRET', '')

    try:
        event = stripe.Webhook.construct_event(
            payload, sig_header, endpoint_secret
        )
    except ValueError:
        return HttpResponse(status=400)
    except stripe.error.SignatureVerificationError:
        return HttpResponse(status=400)

    # Handle eventos
    if event['type'] == 'checkout.session.completed':
        session = event['data']['object']
        handle_checkout_session(session)

    elif event['type'] == 'customer.subscription.updated':
        subscription = event['data']['object']
        handle_subscription_updated(subscription)

    elif event['type'] == 'customer.subscription.deleted':
        subscription = event['data']['object']
        handle_subscription_deleted(subscription)

    return HttpResponse(status=200)


def handle_checkout_session(session):
    """Processa sessão de checkout completada"""
    user_id = session['metadata'].get('user_id')
    plan = session['metadata'].get('plan')

    if user_id:
        from django.contrib.auth.models import User
        try:
            user = User.objects.get(id=user_id)
            profile = user.profile
            profile.plano = plan
            profile.stripe_customer_id = session['customer']
            profile.stripe_subscription_id = session['subscription']
            profile.save()
        except User.DoesNotExist:
            pass


def handle_subscription_updated(subscription):
    """Processa atualização de assinatura"""
    customer_id = subscription['customer']
    status = subscription['status']

    try:
        profile = UserProfile.objects.get(stripe_customer_id=customer_id)

        if status == 'active':
            # Assinatura ativa
            pass
        elif status in ['canceled', 'unpaid', 'past_due']:
            # Downgrade para free
            profile.plano = 'free'
            profile.limite_transacoes = 50
            profile.limite_orcamentos = 3
            profile.save()

    except UserProfile.DoesNotExist:
        pass


def handle_subscription_deleted(subscription):
    """Processa cancelamento de assinatura"""
    customer_id = subscription['customer']

    try:
        profile = UserProfile.objects.get(stripe_customer_id=customer_id)
        profile.plano = 'free'
        profile.limite_transacoes = 50
        profile.limite_orcamentos = 3
        profile.stripe_subscription_id = ''
        profile.save()

    except UserProfile.DoesNotExist:
        pass


@login_required
def customer_portal(request):
    """Redireciona para o portal do cliente Stripe"""
    try:
        profile = request.user.profile

        if not profile.stripe_customer_id:
            return redirect('pricing')

        session = stripe.billing_portal.Session.create(
            customer=profile.stripe_customer_id,
            return_url=request.build_absolute_uri(reverse('dashboard')),
        )

        return redirect(session.url)

    except Exception as e:
        return redirect('dashboard')
